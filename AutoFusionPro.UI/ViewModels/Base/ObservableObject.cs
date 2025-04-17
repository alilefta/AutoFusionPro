using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoFusionPro.UI.ViewModels.Base
{
    /// <summary>
    /// Disabled and unused for now. Using Observable object from MVVM Toolkit
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {

        #region Property Changed Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler? handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            }
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}
