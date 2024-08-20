using PTI.Rs232Validator;
using System;
using System.Collections.ObjectModel;

namespace PyramidNETRS232_TestApp;

// This portion... TODO: Finish.
public partial class MainWindow
{
    public Logger Logger { get; } = new();
    
    private void HighlightSequence(int index)
    {
        LoggerListView.SelectedIndex = index;
    }
    
    private void LoggerListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        HighlightSequence(LoggerListView.SelectedIndex);
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

public class Logger : ILogger
{
    public ObservableCollection<LogEntry> LogEntries { get; } = new();
        
    public void Trace(string format, params object[] args)
    {
        LogEntries.Add(new LogEntry(LogLevel.Trace, DateTimeOffset.Now, string.Format(format, args)));
    }

    public void Debug(string format, params object[] args)
    {
        LogEntries.Add(new LogEntry(LogLevel.Debug, DateTimeOffset.Now, string.Format(format, args)));
    }

    public void Info(string format, params object[] args)
    {
        LogEntries.Add(new LogEntry(LogLevel.Info, DateTimeOffset.Now, string.Format(format, args)));
    }

    public void Error(string format, params object[] args)
    {
        LogEntries.Add(new LogEntry(LogLevel.Error, DateTimeOffset.Now, string.Format(format, args)));
    }
}