namespace AutoFusionPro.Application.Interfaces.Dialogs
{
    public interface IDialogWindow
    {
        bool? DialogResult { get; set; }
        void Close();
    }
}
