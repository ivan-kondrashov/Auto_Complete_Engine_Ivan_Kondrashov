using Microsoft.Win32;
using System.Windows;

namespace AutoCompleteEngine.UI.Services.Dialog;

public class DialogService : IDialogService
{
    public void ShowMessage(string message)
    {
        MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void ShowErrorMessage(string errMessage)
    {
        MessageBox.Show(errMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public void ShowWarningMessage(string warnMessage)
    {
        MessageBox.Show(warnMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    public string OpenCsvDialog()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
            Title = "Select BenchmarkDotNet report"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            return openFileDialog.FileName;
        }

        return string.Empty;
    }
}
