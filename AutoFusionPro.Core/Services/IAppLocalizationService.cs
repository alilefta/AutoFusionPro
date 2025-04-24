using AutoFusionPro.Core.Enums.SystemEnum;

namespace AutoFusionPro.Core.Services
{
    public interface IAppLocalizationService<FD> where FD : Enum
    {
        void ApplyLanguage(Languages language);
        void ApplyTheme(bool isDarkThemeEnabled);

        FD CurrentFlowDirection { get; }
        string CurrentLangString { get; }
        event Action FlowDirectionChanged;
    }
}
