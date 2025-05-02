using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.Parts.DesignTimeViewModels
{
    public class ActiveFilters
    {
        public static ActiveFilters DesignInstance = new ActiveFilters();


        public ObservableCollection<FilterItemViewModel> Filters { get; set;}

        public ActiveFilters()
        {
            Filters = new ObservableCollection<FilterItemViewModel>
            {
                new FilterItemViewModel("Model: 2018, 2019"),
                new FilterItemViewModel("Price: 100 - 150"),
                new FilterItemViewModel("Color: White"),
                new FilterItemViewModel("Category: Engine Parts"),
                new FilterItemViewModel("Category: Engine Parts"),
                new FilterItemViewModel("Category: Engine Parts"),
                new FilterItemViewModel("Category: Engine Parts"),
                new FilterItemViewModel("Category: Engine Parts"),
                new FilterItemViewModel("Category: Engine Parts"),
                new FilterItemViewModel("Category: Engine Parts"),
                new FilterItemViewModel("Category: Engine Parts"),
                new FilterItemViewModel("Category: Engine Parts"),
            };
        }
    }

    public class FilterItemViewModel
    {
        public string Name { get; set; }

        public FilterItemViewModel()
        {
            
        }

        public FilterItemViewModel(string name)
        {
            Name = name;
        }

    }
}
