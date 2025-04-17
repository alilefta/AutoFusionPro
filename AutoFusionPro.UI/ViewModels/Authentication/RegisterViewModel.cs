using AutoFusionPro.UI.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.UI.ViewModels.Authentication
{
    public class RegisterViewModel : BaseViewModel
    {
        public event EventHandler OnRegisterSuccessful;
        public event EventHandler ShowLoginView;

        public RegisterViewModel()
        {
            
        }
    }
}
