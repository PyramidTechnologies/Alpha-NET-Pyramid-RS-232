using PTI.Rs232Validator;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace PyramidNETRS232_TestApp;

// This portion displays the current state and latest event of an Apex validator.
partial class MainWindow
{
    private static readonly SolidColorBrush InactiveBrush = new(Colors.LightGray);
    private static readonly SolidColorBrush CashBoxAttachedBrush = new(Colors.LightYellow);
    private static readonly SolidColorBrush ActiveEventBrush = new(Colors.LightGreen);
    private static readonly SolidColorBrush ActiveStateBrush = new(Colors.LightBlue);

    private Rs232State _state = Rs232State.None;
    private Rs232Event _event = Rs232Event.None;

    /// <summary>
    /// <see cref="Rs232State"/> of <see cref="ApexValidator"/>.
    /// </summary>
    public Rs232State State
    {
        get => _state;
        set
        {
            DoOnUiThread(() =>
            {
                DeactivateButtonsWithTag(_stateTagText);
                switch (value)
                {
                    case Rs232State.Idling:
                        IdlingButton.Background = ActiveStateBrush;
                        break;
                    case Rs232State.Accepting:
                        AcceptingButton.Background = ActiveStateBrush;
                        break;
                    case Rs232State.Escrowed:
                        EscrowedButton.Background = ActiveStateBrush;
                        break;
                    case Rs232State.Stacking:
                        StackingButton.Background = ActiveStateBrush;
                        break;
                    case Rs232State.Returning:
                        ReturningButton.Background = ActiveStateBrush;
                        break;
                    case Rs232State.BillJammed:
                        BillJammedButton.Background = ActiveStateBrush;
                        break;
                    case Rs232State.StackerFull:
                        StackerFullButton.Background = ActiveStateBrush;
                        break;
                    case Rs232State.Failure:
                        FailureButton.Background = ActiveStateBrush;
                        break;
                }
            });

            _state = value;
            NotifyPropertyChanged(nameof(State));
        }
    }

    /// <summary>
    /// <see cref="Rs232Event"/> instances raised by <see cref="ApexValidator"/>.
    /// </summary>
    public Rs232Event Event
    {
        get => _event;
        set
        {
            DoOnUiThread(() =>
            {
                DeactivateButtonsWithTag(_eventTagText);
                switch (value)
                {
                    case Rs232Event.Stacked:
                        StackedButton.Background = ActiveEventBrush;
                        break;
                    case Rs232Event.Returned:
                        ReturnedButton.Background = ActiveEventBrush;
                        break;
                    case Rs232Event.Cheated:
                        CheatedButton.Background = ActiveEventBrush;
                        break;
                    case Rs232Event.BillRejected:
                        RejectedButton.Background = ActiveEventBrush;
                        break;
                    case Rs232Event.PowerUp:
                        Console.WriteLine("The Apex validator has powered up.");
                        break;
                }
            });

            _event = value;
            NotifyPropertyChanged(nameof(Event));
        }
    }

    private void ApexValidator_OnStateChanged(object? sender, StateChangeArgs args)
    {
        State = args.NewState;
    }

    private void ApexValidator_OnEventReported(object? sender, Rs232Event rs232Event)
    {
        Event = rs232Event;
    }

    private void ApexValidator_CashBoxAttached(object? sender, EventArgs e)
    {
        Console.WriteLine("Cash box has been attached.");
        DoOnUiThread(() => CashBoxButton.Background = CashBoxAttachedBrush);
    }

    private void ApexValidator_CashBoxRemoved(object? sender, EventArgs e)
    {
        Console.WriteLine("Cash box has been removed.");
        DoOnUiThread(() => CashBoxButton.Background = InactiveBrush);
    }

    private void DeactivateButtonsWithTag(string tagText)
    {
        var buttons = StateMachine.Children.OfType<Button>();
        foreach (var button in buttons)
        {
            if (button.Tag as string == tagText)
            {
                button.Background = InactiveBrush;
            }
        }
    }
}