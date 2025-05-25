using AutoFusionPro.Core.Enums.UI;

namespace AutoFusionPro.Application.Interfaces.UI
{
    public interface IToastNotificationService
    {
        void Show(string message, string? title = null, ToastType type = ToastType.Info, TimeSpan? duration = null);
        void ShowSuccess(string message, string? title = null, TimeSpan? duration = null);
        void ShowError(string message, string? title = null, TimeSpan? duration = null);
        void ShowWarning(string message, string? title = null, TimeSpan? duration = null);
        void ShowInfo(string message, string? title = null, TimeSpan? duration = null);
    }
}
