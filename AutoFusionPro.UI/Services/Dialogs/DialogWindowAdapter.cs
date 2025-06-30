using AutoFusionPro.Application.Interfaces.Dialogs;
using System.Windows;

namespace AutoFusionPro.UI.Services.Dialogs
{
    /// <summary>
    /// Dialog Window Adapter without Animations
    /// </summary>
    public class DialogWindowAdapter : IDialogWindow
    {
        private readonly Window _window;
        public Window CurrentDialogInstance => _window;


        public DialogWindowAdapter(Window window)
        {
            _window = window;
        }

        public bool? DialogResult
        {
            get => _window.DialogResult;
            set => _window.DialogResult = value;
        }


        public void Close()
        {
            _window.Close();
        }
    }

}
