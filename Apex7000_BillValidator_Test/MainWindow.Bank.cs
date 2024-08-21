using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PyramidNETRS232_TestApp;

// This portion defines USD bill counts and their cumulative total.
partial class MainWindow
{
    #region Fields

    private static readonly ReadOnlyDictionary<int, int> UsdBillValues = new Dictionary<int, int>
    {
        { 1, 1 },
        { 2, 2 },
        { 3, 5 },
        { 4, 10 },
        { 5, 20 },
        { 6, 50 },
        { 7, 100 }
    }.AsReadOnly();

    private int _bill1Count;
    private int _bill2Count;
    private int _bill3Count;
    private int _bill4Count;
    private int _bill5Count;
    private int _bill6Count;
    private int _bill7Count;
    private int _total;

    #endregion

    #region Properties

    /// <summary>
    /// Count of bill type 1.
    /// </summary>
    public int Bill1Count
    {
        get => _bill1Count;
        set
        {
            _bill1Count = value;
            NotifyPropertyChanged(nameof(Bill1Count));
        }
    }

    /// <summary>
    /// Count of bill type 2.
    /// </summary>
    public int Bill2Count
    {
        get => _bill2Count;
        set
        {
            _bill2Count = value;
            NotifyPropertyChanged(nameof(Bill2Count));
        }
    }

    /// <summary>
    /// Count of bill type 3.
    /// </summary>
    public int Bill3Count
    {
        get => _bill3Count;
        set
        {
            _bill3Count = value;
            NotifyPropertyChanged(nameof(Bill3Count));
        }
    }

    /// <summary>
    /// Count of bill type 4.
    /// </summary>
    public int Bill4Count
    {
        get => _bill4Count;
        set
        {
            _bill4Count = value;
            NotifyPropertyChanged(nameof(Bill4Count));
        }
    }

    /// <summary>
    /// Count of bill type 5.
    /// </summary>
    public int Bill5Count
    {
        get => _bill5Count;
        set
        {
            _bill5Count = value;
            NotifyPropertyChanged(nameof(Bill5Count));
        }
    }

    /// <summary>
    /// Count of bill type 6.
    /// </summary>
    public int Bill6Count
    {
        get => _bill6Count;
        set
        {
            _bill6Count = value;
            NotifyPropertyChanged(nameof(Bill6Count));
        }
    }

    /// <summary>
    /// Count of bill type 7.
    /// </summary>
    public int Bill7Count
    {
        get => _bill7Count;
        set
        {
            _bill7Count = value;
            NotifyPropertyChanged(nameof(Bill7Count));
        }
    }

    /// <summary>
    /// Cumulative total of all bills in US dollars.
    /// </summary>
    public int Total
    {
        get => _total;
        set
        {
            _total = value;
            NotifyPropertyChanged(nameof(Total));
        }
    }

    #endregion

    /// <summary>
    /// Increments the bill count and total when a credit is reported.
    /// </summary>
    private void ApexValidator_OnCreditIndexReported(object? sender, int creditIndex)
    {
        switch (creditIndex)
        {
            case 1:
                Bill1Count++;
                break;
            case 2:
                Bill2Count++;
                break;
            case 3:
                Bill3Count++;
                break;
            case 4:
                Bill4Count++;
                break;
            case 5:
                Bill5Count++;
                break;
            case 6:
                Bill6Count++;
                break;
            case 7:
                Bill7Count++;
                break;
            default:
                Console.WriteLine("Failed to add value of bill {0}.", creditIndex);
                return;
        }

        var value = UsdBillValues[creditIndex];
        Total += value;
        Console.WriteLine("Received bill {0} and added ${1} to total", creditIndex, value);
    }
}