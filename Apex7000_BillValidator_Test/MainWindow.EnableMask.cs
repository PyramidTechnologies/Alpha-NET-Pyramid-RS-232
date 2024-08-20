using System.Windows;

namespace PyramidNETRS232_TestApp;

// This portion
public partial class MainWindow
{
    private void EnabledCheckbox_Changed(object sender, RoutedEventArgs e)
    {
        if (Rs232Config is null)
        {
            return;
        }

        var enableMask = 0;
        enableMask |= EnabledCheckbox1.IsChecked is not null && EnabledCheckbox1.IsChecked.Value ? 1 << 0 : 0;
        enableMask |= EnabledCheckbox2.IsChecked is not null && EnabledCheckbox2.IsChecked.Value ? 1 << 1 : 0;
        enableMask |= EnabledCheckbox3.IsChecked is not null && EnabledCheckbox3.IsChecked.Value ? 1 << 2 : 0;
        enableMask |= EnabledCheckbox4.IsChecked is not null && EnabledCheckbox4.IsChecked.Value ? 1 << 3 : 0;
        enableMask |= EnabledCheckbox5.IsChecked is not null && EnabledCheckbox5.IsChecked.Value ? 1 << 4 : 0;
        enableMask |= EnabledCheckbox6.IsChecked is not null && EnabledCheckbox6.IsChecked.Value ? 1 << 5 : 0;
        enableMask |= EnabledCheckbox7.IsChecked is not null && EnabledCheckbox7.IsChecked.Value ? 1 << 6 : 0;
        
        Rs232Config.EnableMask = (byte)enableMask;
    }
}