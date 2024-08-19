using PTI.Rs232Validator;
using System;

namespace PyramidNETRS232_TestApp;

// This portion processes state and event messages.
partial class MainWindow
{
    private void ApexValidatorOnStateChanged(object sender, StateChangeArgs args)
    {
        State = args.NewState;
    }

    private void Validator_OnEvent(object sender, Rs232Event rs232Event)
    {
        Event = rs232Event;
    }
    
    private void Validator_CashBoxAttached(object sender, EventArgs e)
    {
        Console.WriteLine("Cash box has been attached.");
        SetState(btnCB);
    }
}