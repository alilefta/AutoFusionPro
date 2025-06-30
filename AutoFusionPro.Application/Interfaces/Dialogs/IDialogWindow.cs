using System.Windows;

namespace AutoFusionPro.Application.Interfaces.Dialogs
{
    public interface IDialogWindow
    {
        Window CurrentDialogInstance{ get; }
        bool? DialogResult { get; set; }
        void Close();
    }
}
