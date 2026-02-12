namespace AutoCompleteEngine.UI.Services.Dialog;

public interface IDialogService
{
    string OpenCsvDialog();
    void ShowErrorMessage(string errMessage);
    void ShowMessage(string message);
    void ShowWarningMessage(string warnMessage);
}
