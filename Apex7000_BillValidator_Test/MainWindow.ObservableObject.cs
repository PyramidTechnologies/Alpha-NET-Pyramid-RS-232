using PTI.Rs232Validator;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace PyramidNETRS232_TestApp;

// TODO: Move INotifyPropertyChanged logic to its own file.

internal enum TagType
{
    NonEmptyTags,
    Events,
    States
}

// TODO: Finish.
// This portion...
partial class MainWindow : INotifyPropertyChanged
{
    private static readonly SolidColorBrush InactiveBrush = new(Colors.LightGray);
    private static readonly SolidColorBrush CashBoxOkayBrush = new(Colors.LightYellow);
    private static readonly SolidColorBrush ActiveErrorBrush = new(Colors.LightPink);
    private static readonly SolidColorBrush ActiveEventBrush = new(Colors.LightGreen);
    private static readonly SolidColorBrush ActiveStateBrush = new(Colors.LightBlue);

    #region Fields

    private string _portName = "";
    private Rs232State _state = Rs232State.None;
    private Rs232Event _event = Rs232Event.None;
    private bool _isEscrowMode;
    private bool _isConnected;

    #endregion

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    #region Properties

    public string PortName
    {
        get => _portName;
        set
        {
            _portName = value;
            NotifyPropertyChanged(nameof(PortName));
        }
    }

    public Rs232State State
    {
        get => _state;
        set
        {
            DoOnUIThread(() =>
            {
                switch (value)
                {
                    case Rs232State.Idling:
                        SetState(btnIdle);
                        break;
                    case Rs232State.Accepting:
                        SetState(btnAccepting);
                        break;
                    case Rs232State.Escrowed:
                        SetState(btnEscrowed);
                        break;
                    case Rs232State.Stacking:
                        SetState(btnStacking);
                        break;
                    case Rs232State.Returning:
                        SetState(btnReturning);
                        break;
                    case Rs232State.BillJammed:
                        SetState(btnBillJammed);
                        break;
                    case Rs232State.StackerFull:
                        SetState(btnStackerFull);
                        break;
                    case Rs232State.Failure:
                        SetState(btnFailure);
                        break;
                }
            });

            _state = value;
            NotifyPropertyChanged(nameof(State));
        }
    }

    public Rs232Event Event
    {
        get => _event;
        set
        {
            DoOnUIThread(() =>
            {
                if ((_event
                     & (Rs232Event.Returned | Rs232Event.Stacked | Rs232Event.BillRejected | Rs232Event.Cheated)) != 0)
                {
                    ClearTags(TagType.NonEmptyTags);
                }
            });

            switch (value)
            {
                case Rs232Event.BillRejected:
                    SetActiveEvent(btnRejected);
                    break;
                case Rs232Event.Cheated:
                    SetActiveEvent(btnCheated);
                    break;
                case Rs232Event.PowerUp:
                    Console.WriteLine("Powered Up");
                    break;
                case Rs232Event.Returned:
                    SetActiveEvent(btnReturned);
                    break;
                case Rs232Event.Stacked:
                    SetActiveEvent(btnStacked);
                    break;
                default:
                    SetActiveEvent(null);
                    break;
            }

            _event = value;
        }
    }

    public bool IsEscrowMode
    {
        get => _rsr232Config?.IsEscrowMode ?? _isEscrowMode;
        set
        {
            if (_rsr232Config is not null)
            {
                _rsr232Config.IsEscrowMode = value;
            }
            else
            {
                _isEscrowMode = value;
            }

            NotifyPropertyChanged(nameof(IsEscrowMode));
        }
    }

    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            _isConnected = value;
            NotifyPropertyChanged(nameof(IsConnected));
        }
    }

    #endregion

    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// TODO: Alter summary.
    /// Sets the state tag as active while clearing all other state tags.
    /// </summary>
    private void SetState(Button target)
    {
        DoOnUIThread(() =>
        {
            target.Background = target == btnCB
                ? CashBoxOkayBrush
                : ActiveStateBrush;
        });
    }

    /// <summary>
    /// TODO: Alter summary.
    /// Sets an event as active.
    /// </summary>
    private void SetActiveEvent(Button? target)
    {
        DoOnUIThread(() =>
        {
            if (target is not null)
            {
                target.Background = ActiveEventBrush;
            }
            else
            {
                ClearTags(TagType.Events);
            }
        });
    }

    /// <summary>
    /// TODO: Alter summary.
    /// Sets an error as active.
    /// </summary>
    private void SetError(Button target)
    {
        DoOnUIThread(() =>
        {
            ClearTags(TagType.NonEmptyTags);
            target.Background = ActiveErrorBrush;
        });
    }

    /// <summary>
    /// TODO: Alter summary.
    /// Resets all state tags back to lightGrey. Must be called from UI thread.
    /// </summary>
    private void ClearTags(TagType type)
    {
        var stateTags = StateMachine.Children.OfType<Button>();
        foreach (var button in stateTags)
        {
            var tag = button.Tag is null
                ? ""
                : button.Tag.ToString() ?? "";
            switch (type)
            {
                case TagType.NonEmptyTags:
                    if (!string.IsNullOrEmpty(tag))
                    {
                        button.Background = InactiveBrush;
                    }

                    break;
                case TagType.Events:
                    if (tag.Equals("event"))
                    {
                        button.Background = InactiveBrush;
                    }

                    break;
                case TagType.States:
                    if (tag.Equals("state"))
                    {
                        button.Background = InactiveBrush;
                    }

                    break;
            }
        }
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
}