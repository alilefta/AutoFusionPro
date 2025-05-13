namespace AutoFusionPro.UI.ViewModels.Base
{
    public interface ITabViewModel
    {
        public bool IsVisible { get; }
        public bool IsLoading { get; set; }

        public string DisplayName { get; set; }
        public string Icon { get; set; }
    }
}
