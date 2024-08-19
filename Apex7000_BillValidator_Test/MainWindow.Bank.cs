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
    internal int Bill1Count
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
    internal int Bill2Count
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
    internal int Bill3Count
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
    internal int Bill4Count
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
    internal int Bill5Count
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
    internal int Bill6Count
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
    internal int Bill7Count
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
    internal int Total
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
    /// Adds value of specified bill to the total and returns the value.
    /// </summary>
    internal int AddBillValue(int bill)
    {
        switch (bill)
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
                // An illegal value was received.
                return 0;
        }

        var value = UsdBillValues[bill];
        Total += value;
        return value;
    }
}