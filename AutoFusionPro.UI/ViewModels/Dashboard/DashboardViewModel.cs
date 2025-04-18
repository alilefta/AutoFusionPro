using AutoFusionPro.Application.Commands;
using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Enums.UI;
using AutoFusionPro.UI.ViewModels.Base;
using System.Windows.Input;

namespace AutoFusionPro.UI.ViewModels.Dashboard
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly IWpfToastNotificationService _toastNotificationService;

        public ICommand ShowToast { get; set; }

        public DashboardViewModel(IWpfToastNotificationService toastNotificationService)
        {
            _toastNotificationService = toastNotificationService ?? throw new ArgumentNullException(nameof(toastNotificationService));
            ShowToast = new RelayCommand(o => ShowToastCommand(o), o => true);
        }


        private void ShowToastCommand(object? param = null)
        {
            if (param is null) return;

            var type = (ToastType)param;
            _toastNotificationService.Show("اختبار ناجح ربما", "اختبار", type, TimeSpan.FromSeconds(5));
            _toastNotificationService.Show("Test", "Test", type, TimeSpan.FromSeconds(5));





        }
    }
}
