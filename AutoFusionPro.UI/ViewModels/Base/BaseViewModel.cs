using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace AutoFusionPro.UI.ViewModels.Base
{
    public partial class BaseViewModel: ObservableValidator, IDisposable
    {
        private readonly List<Action> _cleanupActions = new();

        #region Flow Direction

        [ObservableProperty]
        private FlowDirection _currentWorkFlow;

        #endregion

        #region Data Error

        public string Error => throw new NotImplementedException();


        #endregion

        /// <summary>
        /// Registers an event unsubscription action.
        /// </summary>
        protected void RegisterCleanup(Action cleanupAction)
        {
            _cleanupActions.Add(cleanupAction);
        }


        public void Dispose()
        {
            foreach (var action in _cleanupActions)
            {
                action.Invoke();
            }
            _cleanupActions.Clear();
        }

    }
}
