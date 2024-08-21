using PTI.Rs232Validator;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace PyramidNETRS232_TestApp;

// This portion provides logging.
public partial class MainWindow : ILogger
{
    public ObservableCollection<LogEntry> LogEntries { get; } = [];
        
    public void Trace(string format, params object[] args)
    {
        // Do nothing.
    }

    public void Debug(string format, params object[] args)
    {
        DoOnUiThread(() =>
        {
            LogEntries.Add(new LogEntry(LogLevel.Debug, DateTimeOffset.Now, string.Format(format, args)));
        });
    }

    public void Info(string format, params object[] args)
    {
        DoOnUiThread(() =>
        {
            LogEntries.Add(new LogEntry(LogLevel.Info, DateTimeOffset.Now, string.Format(format, args)));
        });
    }

    public void Error(string format, params object[] args)
    {
        DoOnUiThread(() =>
        {
            LogEntries.Add(new LogEntry(LogLevel.Error, DateTimeOffset.Now, string.Format(format, args)));
        });
    }
    
    private void LoggerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoggerListView.SelectedIndex = LoggerListView.SelectedIndex;
    }
}

public enum LogLevel
{
    Trace,
    Debug,
    Info,
    Error
}
    
public record LogEntry(LogLevel Level, DateTimeOffset Timestamp, string Message);