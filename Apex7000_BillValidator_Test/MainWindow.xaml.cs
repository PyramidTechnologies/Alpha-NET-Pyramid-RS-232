using PTI.Rs232Validator;
using System;
using System.IO.Ports;
using System.Windows;

namespace PyramidNETRS232_TestApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private Rs232Config? _rsr232Config;
    private ApexValidator? _apexValidator;
    
    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();
    }

    /// Simple UI listeners

    private void btnReset_Click(object sender, RoutedEventArgs e)
    {
        if (_apexValidator is null)
        {
            return;
        }
        
        _apexValidator.StopPollingLoop();
        _apexValidator.StartPollingLoop();
    }


    private void chkEscrowMode_Checked(object sender, RoutedEventArgs e)
    {
        IsEscrowMode = true;
    }

    private void chkEscrowMode_Unchecked(object sender, RoutedEventArgs e)
    {
        IsEscrowMode = false;
    }

    private void AvailablePorts_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        AvailablePorts.ItemsSource = SerialPort.GetPortNames();
    }

    private void AvailablePorts_Loaded(object sender, RoutedEventArgs e)
    {
        AvailablePorts.ItemsSource = SerialPort.GetPortNames();
    }

    private void ed_Changed(object sender, RoutedEventArgs e)
    {
        if (_rsr232Config is null)
        {
            return;
        }

        var enableMask = 0;
        enableMask |= chk1.IsChecked is not null && chk1.IsChecked.Value ? 1 << 0 : 0;
        enableMask |= chk2.IsChecked is not null && chk2.IsChecked.Value ? 1 << 1 : 0;
        enableMask |= chk3.IsChecked is not null && chk3.IsChecked.Value ? 1 << 2 : 0;
        enableMask |= chk4.IsChecked is not null && chk4.IsChecked.Value ? 1 << 3 : 0;
        enableMask |= chk5.IsChecked is not null && chk5.IsChecked.Value ? 1 << 4 : 0;
        enableMask |= chk6.IsChecked is not null && chk6.IsChecked.Value ? 1 << 5 : 0;
        enableMask |= chk7.IsChecked is not null && chk7.IsChecked.Value ? 1 << 6 : 0;
        
        _rsr232Config.EnableMask = (byte)enableMask;
    }

    private void sldPoll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_rsr232Config == null || txtPoll == null)
        {
            return;
        }

        var ms = (int)e.NewValue;
        _rsr232Config.PollingPeriod = TimeSpan.FromMicroseconds(ms);
        txtPoll.Text = $"{ms} ms";
    }

    private void HighlightSequence(int index)
    {
        ConsoleLogger.SelectedIndex = index;
    }

    private void ConsoleLogger_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        HighlightSequence(ConsoleLogger.SelectedIndex);
    }
}