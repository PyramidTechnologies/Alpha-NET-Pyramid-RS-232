using PTI.Rs232Validator;
using PTI.Rs232Validator.Providers;
using System.Windows;

namespace PyramidNETRS232_TestApp;

// This portion connects to an RS-232 bill acceptor.
partial class MainWindow
{
    /// <summary>
    /// Occurs when users clicks connect. Once in the connected state, the 
    /// buttons becomes a disconnect button. Clicking in the disconnected state
    /// will connect, clicking in the connected will disconnect.
    /// </summary>
    private void btnConnect_Click(object sender, RoutedEventArgs e)
    {
        if (IsConnected)
        {
            _apexValidator?.StopPollingLoop();
            btnConnect.Content = "Connect";
            IsConnected = false;
            return;
        }
        
        PortName = AvailablePorts.Text;
        // TODO: Make string a constant.
        if (string.IsNullOrEmpty(PortName) || PortName == "Select Port")
        {
            MessageBox.Show("Please select a port.");
            return;
        }
        
        // Instantiate a validator and register for all the handlers we need
        IsConnected = true;
        btnConnect.Content = "Disconnect";

        // Create a new instance using the specified port and in escrow mode
        var usbSerialProvider = new UsbSerialProvider(PortName);
        _rsr232Config = new Rs232Config(PortName, IsEscrowMode);
        _apexValidator = new PyramidAcceptor(_rsr232Config);
        _apexValidator = new ApexValidator()

        // Configure events and state (All optional) - see StateAndEvents_Sample.cs
        _apexValidator.OnEvent += Validator_OnEvent;
        _apexValidator.OnStateChanged += ApexValidatorOnStateChanged;
        _apexValidator.OnError += validator_OnError;
        _apexValidator.OnCashboxAttached += Validator_CashBoxAttached;

        // Required if you are in escrow mode - see CreditAndEscrow_Sample.cs
        _apexValidator.OnEscrow += validator_OnEscrow;
        _apexValidator.OnBillInEscrow += validator_OnBillInEscrow;

        // Technically optional but you probably want this event - see CreditAndEscrow_Sample.cs
        _apexValidator.OnCredit += validator_OnCredit;

        // This starts the acceptor - REQUIRED!!
        _apexValidator.Connect();
    }

    private void btnResumePause_Click(object sender, RoutedEventArgs e)
    {
        if (_apexValidator.IsPaused)
        {
            _apexValidator.ResumeAcceptance();
            _apexValidator.ResmeAcceptance();
            btnResumePause.Content = "Pause";
        } 
        else
        {
            _apexValidator.PauseAcceptance();
            btnResumePause.Content = "Resume";
        }
    }
}