using PTI.Rs232Validator;
using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PyramidNETRS232_TestApp;

// This portion implements INotifyPropertyChanged and establishes a connection to an Apex validator.
/// <summary>
/// Main window of application.
/// </summary>
public partial class MainWindow : INotifyPropertyChanged
{
    public MainWindow()
    {
        InitializeComponent();

        _selectPortText = FindResource("SelectPortText") as string ??
                          throw new InvalidOperationException("String resource with key 'SelectPortText' not found.");
        _connectText = FindResource("ConnectText") as string ??
                       throw new InvalidOperationException("String resource with key 'ConnectText' not found.");
        _disconnectText = FindResource("DisconnectText") as string ??
                          throw new InvalidOperationException("String resource with key 'DisconnectText' not found.");
        _pauseText = FindResource("PauseText") as string ??
                     throw new InvalidOperationException("String resource with key 'PauseText' not found.");
        _resumeText = FindResource("ResumeText") as string ??
                      throw new InvalidOperationException("String resource with key 'ResumeText' not found.");
        _stateTagText = FindResource("StateTagText") as string ??
                        throw new InvalidOperationException("String resource with key 'StateTagText' not found.");
        _eventTagText = FindResource("EventTagText") as string ??
                        throw new InvalidOperationException("String resource with key 'EventTagText' not found.");
    }

    private readonly string _selectPortText;
    private readonly string _connectText;
    private readonly string _disconnectText;
    private readonly string _pauseText;
    private readonly string _resumeText;
    private readonly string _stateTagText;
    private readonly string _eventTagText;

    private string _portName = string.Empty;
    private bool _isConnected;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;


    /// <summary>
    /// Is there an established connection to an Apex validator?
    /// </summary>
    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            DoOnUiThread(() =>
            {
                ConnectButton.Content = value ? _disconnectText : _connectText;
            });
            
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

    private void DoOnUiThread(Action action)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.InvokeAsync(action);
        }
        else
        {
            action.Invoke();
        }
    }

    private void AvailablePorts_MouseLeave(object sender, MouseEventArgs e)
    {
        AvailablePortsComboBox.ItemsSource = SerialPort.GetPortNames();
    }

    private void AvailablePorts_Loaded(object sender, RoutedEventArgs e)
    {
        AvailablePortsComboBox.ItemsSource = SerialPort.GetPortNames();
    }

    /// <summary>
    /// Connects to an Apex validator on the selected port.
    /// </summary>
    private void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        if (IsConnected)
        {
            ApexValidator?.StopPollingLoop();
            IsConnected = false;
            ConnectButton.Content = _connectText;
            return;
        }

        if (string.IsNullOrEmpty(AvailablePortsComboBox.Text) || AvailablePortsComboBox.Text == _selectPortText)
        {
            MessageBox.Show("Please select a port.");
            return;
        }

        if (_portName != AvailablePortsComboBox.Text)
        {
            ApexValidator?.StopPollingLoop();
            ApexValidator?.Dispose();
        }

        _portName = AvailablePortsComboBox.Text;
        Rs232Config = Rs232Config.UsbRs232Config(_portName, this);
        ApexValidator = new ApexValidator(Rs232Config);
        ApexValidator.OnLostConnection += ApexValidator_OnLostConnection;

        // Visit MainWindow.StatesAndEvents.cs for more information.
        // ApexValidator.OnStateChanged += ApexValidator_OnStateChanged;
        // ApexValidator.OnEventReported += ApexValidator_OnEventReported;
        // ApexValidator.OnCashBoxAttached += ApexValidator_CashBoxAttached;
        // ApexValidator.OnCashBoxRemoved += ApexValidator_CashBoxRemoved;

        // Visit MainWindow.Escrow.cs for more information.
        IsEscrowMode = true;
        // ApexValidator.OnBillInEscrow += ApexValidator_OnBillInEscrow;

        // Visit MainWindow.Bank for more information.
        // ApexValidator.OnCreditIndexReported += ApexValidator_OnCreditIndexReported;

        // Start the RS232 polling loop.
        IsConnected = ApexValidator.StartPollingLoop();
        if (!IsConnected)
        {
            ApexValidator.StopPollingLoop();
            ApexValidator.Dispose();
            MessageBox.Show("Failed to connect to the Apex validator.");
        }
    }

    /// <summary>
    /// Signals that the connection to the Apex validator has been lost.
    /// </summary>
    private void ApexValidator_OnLostConnection(object? sender, EventArgs e)
    {
        IsConnected = false;
    }

    /// <summary>
    /// Pauses or resumes the acceptance of bills.
    /// </summary>
    private void PauseResumeButton_Click(object sender, RoutedEventArgs e)
    {
        if (ApexValidator is null)
        {
            return;
        }

        if (ApexValidator.IsPaused)
        {
            ApexValidator.ResumeAcceptance();
            PauseResumeButton.Content = _pauseText;
        }
        else
        {
            ApexValidator.PauseAcceptance();
            PauseResumeButton.Content = _resumeText;
        }
    }

    /// <summary>
    /// Resets <see cref="ApexValidator"/>.
    /// </summary>
    private async void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        if (ApexValidator is null || Rs232Config is null)
        {
            return;
        }

        ApexValidator.StopPollingLoop();
        await Task.Delay(Rs232Config.PollingPeriod.Milliseconds * 2);
        IsConnected = ApexValidator.StartPollingLoop();
    }

    /// <summary>
    /// Alters the polling period of <see cref="Rs232Config"/>.
    /// </summary>
    private void PollSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (Rs232Config is null)
        {
            return;
        }

        var ms = (int)e.NewValue;
        Rs232Config.PollingPeriod = TimeSpan.FromMilliseconds(ms);
        PollTextBox.Text = $"{ms} ms";
    }
}