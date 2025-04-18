﻿using AutoFusionPro.Core.Enums.NavigationPages;

namespace AutoFusionPro.Application.Interfaces
{
    public interface IViewModelFactory
    {
        object ResolveViewModel(ApplicationPage page);
        object ResolveViewModelWithInitialization(ApplicationPage page, object parameter);

        object GetInitializationWrapperClass();
    }
}
