using AutoFusionPro.Core.Enums.SystemEnum;
using System.Windows;

namespace AutoFusionPro.Core.Services
{
    public interface ILocalizationService<FD> where FD : Enum
    {
        void ApplyLanguage(Languages language);
        void ApplyTheme(bool isDarkThemeEnabled);

        FD CurrentFlowDirection { get; }
        string CurrentLangString { get; }
        event Action FlowDirectionChanged;
    }
}
