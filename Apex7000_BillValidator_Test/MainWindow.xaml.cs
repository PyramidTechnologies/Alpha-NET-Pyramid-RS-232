using PTI.Rs232Validator;
using PTI.Rs232Validator.Providers;
using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Input;

namespace PyramidNETRS232_TestApp;

// This portion implements INotifyPropertyChanged and handles the connection to an RS-232 bill acceptor.
/// <summary>
/// Main window of application.
/// </summary>
public partial class MainWindow : INotifyPropertyChanged
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public const string SelectPortText = "Select Port";
    public const string ConnectText = "Connect";
    public const string DisconnectText = "Disconnect";
    public const string PauseText = "Pause";
    public const string ResumeText = "Resume";

    private string _portName = string.Empty;
    private bool _isConnected;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            _isConnected = value;
            NotifyPropertyChanged(nameof(IsConnected));
        }
    }

    /// <inheritdoc cref="PTI.Rs232Validator.Rs232Config"/>
    internal Rs232Config? Rs232Config { get; set; }

    /// <inheritdoc cref="PTI.Rs232Validator.ApexValidator"/>
    internal ApexValidator? ApexValidator { get; set; }

    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void DoOnUIThread(Action action)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(action);
        }
        else
        {
            action.Invoke();
        }
    }

    private void AvailablePorts_MouseLeave(object sender, MouseEventArgs e)
    {
        AvailablePorts.ItemsSource = SerialPort.GetPortNames();
    }

    private void AvailablePorts_Loaded(object sender, RoutedEventArgs e)
    {
        AvailablePorts.ItemsSource = SerialPort.GetPortNames();
    }

    private void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        if (IsConnected)
        {
            ApexValidator?.StopPollingLoop();
            IsConnected = false;
            ConnectButton.Content = ConnectText;
            return;
        }
        
        if (string.IsNullOrEmpty(AvailablePorts.Text) || AvailablePorts.Text == SelectPortText)
        {
            MessageBox.Show("Please select a port.");
            return;
        }
        
        if (_portName != AvailablePorts.Text)
        {
            ApexValidator?.StopPollingLoop();
            ApexValidator?.Dispose();
        }
        
        _portName = AvailablePorts.Text;
        var usbSerialProvider = new UsbSerialProvider(_portName);
        Rs232Config = new Rs232Config(usbSerialProvider, Logger)
        {
            IsEscrowMode = IsEscrowMode
        };
        ApexValidator = new ApexValidator(Rs232Config);

        // Visit MainWindow.StatesAndEvents.cs for more information.
        ApexValidator.OnStateChanged += ApexValidator_OnStateChanged;
        ApexValidator.OnEventReported += ApexValidator_OnEventReported;
        ApexValidator.OnCashBoxAttached += Validator_CashBoxAttached;
        ApexValidator.OnCashBoxRemoved += Validator_CashBoxRemoved;

        // Visit MainWindow.Escrow.cs for more information.
        IsEscrowMode = true;
        ApexValidator.OnBillInEscrow += ApexValidator_OnBillInEscrow;

        // Visit MainWindow.Bank for more information.
        ApexValidator.OnCreditIndexReported += ApexValidator_OnCreditIndexReported;

        // Start the RS232 polling loop.
        IsConnected = ApexValidator.StartPollingLoop();
        if (IsConnected)
        {
            ConnectButton.Content = DisconnectText;
        }
        else
        {
            MessageBox.Show("Failed to connect to the Apex validator.");
        }
    }

    private void PauseButton_Click(object sender, RoutedEventArgs e)
    {
        if (ApexValidator is null)
        {
            return;
        }

        if (ApexValidator.IsPaused)
        {
            ApexValidator.ResumeAcceptance();
            btnResumePause.Content = PauseText;
        }
        else
        {
            ApexValidator.PauseAcceptance();
            btnResumePause.Content = ResumeText;
        }
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        if (ApexValidator is null)
        {
            return;
        }

        ApexValidator.StopPollingLoop();
        ApexValidator.StartPollingLoop();
    }
    
    private void PollSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (Rs232Config is null)
        {
            return;
        }

        var ms = (int)e.NewValue;
        Rs232Config.PollingPeriod = TimeSpan.FromMicroseconds(ms);
        PollTextBox.Text = $"{ms} ms";
    }
}