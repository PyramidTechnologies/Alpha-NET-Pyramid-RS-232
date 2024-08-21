using System;
using System.Windows;

namespace PyramidNETRS232_TestApp;

// This portion processes bills in escrow.
partial class MainWindow
{
    private static readonly object ManualLock = new();
    private bool _isEscrowMode;
    private bool _billIsInEscrow;
    private int _lastCreditIndex;

    /// <summary>
    /// Is <see cref="ApexValidator"/> in escrow mode?
    /// </summary>
    public bool IsEscrowMode
    {
        get => Rs232Config?.IsEscrowMode ?? _isEscrowMode;
        set
        {
            if (Rs232Config is not null)
            {
                Rs232Config.IsEscrowMode = value;
            }
            else
            {
                _isEscrowMode = value;
            }

            NotifyPropertyChanged(nameof(IsEscrowMode));
        }
    }

    /// <summary>
    /// Is a bill in escrow?
    /// </summary>
    public bool BillIsInEscrow
    {
        get => _billIsInEscrow;
        set
        {
            _billIsInEscrow = value;
            NotifyPropertyChanged(nameof(BillIsInEscrow));
        }
    }

    /// <summary>
    /// Prepares to handle a bill in escrow.
    /// </summary>
    private void ApexValidator_OnBillInEscrow(object? sender, int creditIndex)
    {
        _lastCreditIndex = creditIndex;
        Console.WriteLine("Received bill {0} in escrow.", _lastCreditIndex);

        DoOnUiThread(() =>
        {
            // Rejects are triggered by:
            // 1) Invalid note
            // 2) Cheat attempt

            // Returns are triggered by:
            // 1) Note disabled by E/D mask
            // 2) Manual delivery of return message because a check failed (e.g. too much money in user account)

            lock (ManualLock)
            {
                BillIsInEscrow = true;
            }
        });
    }

    private void EscrowCheckbox_Checked(object sender, RoutedEventArgs e)
    {
        IsEscrowMode = true;
    }

    private void EscrowCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
        IsEscrowMode = false;
    }

    /// <summary>
    /// Notifies <see cref="ApexValidator"/> to stack the bill in escrow.
    /// </summary>
    private void StackButton_Click(object sender, RoutedEventArgs e)
    {
        if (ApexValidator is null)
        {
            return;
        }

        ApexValidator.Stack();
        if (UsdBillValues.TryGetValue(_lastCreditIndex, out int value))
        {
            Console.WriteLine("Stacked ${0}.", value);
        }
        else
        {
            Console.WriteLine("Stacked unknown bill {0}.", _lastCreditIndex);
        }

        lock (ManualLock)
        {
            _lastCreditIndex = 0;
            BillIsInEscrow = false;
        }
    }

    /// <summary>
    /// Notifies <see cref="ApexValidator"/> to return the bill in escrow.
    /// </summary>
    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        if (ApexValidator is null)
        {
            return;
        }

        ApexValidator.Return();
        if (UsdBillValues.TryGetValue(_lastCreditIndex, out var value))
        {
            Console.WriteLine("Returned ${0}.", value);
        }
        else
        {
            Console.WriteLine("Returned unknown bill {0}.", _lastCreditIndex);
        }

        lock (ManualLock)
        {
            _lastCreditIndex = 0;
            BillIsInEscrow = false;
        }
    }
}