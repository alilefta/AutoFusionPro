using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.UI.ViewModels.Categories.Dialogs
{
    public class EditRootCategoryDialogViewModel: InitializableViewModel<EditRootCategoryDialogViewModel>, IDialogAware
    {

        public EditRootCategoryDialogViewModel(ILocalizationService localizationService, ILogger<EditRootCategoryDialogViewModel> logger) : base(localizationService, logger)
        {

        }

        public void SetDialogWindow(IDialogWindow dialog)
        {
            throw new NotImplementedException();
        }
    }
}
