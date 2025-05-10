using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.Parts.Dialogs
{
    public partial class AddPartDialogViewModel : BaseViewModel<AddPartDialogViewModel>
    {
        private readonly IPartService _partService;

        [ObservableProperty]
        private bool _isAddingPart = false;

        #region Part Properties

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _partNumber = string.Empty;


        #endregion

        public AddPartDialogViewModel(IPartService partService, 
            ILocalizationService localizationService, 
            ILogger<AddPartDialogViewModel> logger) : base(localizationService, logger)
        {
            _partService = partService ?? throw new ArgumentNullException(nameof(partService));
        }


    }
}
