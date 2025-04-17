using System.Windows.Input;

namespace AutoFusionPro.Application.Commands
{
    public class RelayCommand : ICommand
    {
        #region private Members
        /// <summary>
        /// Action to run
        /// </summary>
        private readonly Action<object> _action;

        private readonly Predicate<object> _canExecute;


        #endregion

        #region Public Command
        /// <summary>
        /// The event that fires when the <see cref="CanExecute(object?)"/> value has changed
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        #endregion

        #region Constructor
        /// <summary>
        /// a Default constructor
        /// </summary>
        /// <param name="action"></param>
        public RelayCommand(Action<object> action, Predicate<object> canExecute)
        {
            _action = action ?? throw new ArgumentNullException(nameof(_action));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(_canExecute));
        }

        #endregion

        #region Command Method
        // This function for example used to gray out buttons that cannot execute
        /// <summary>
        /// A relay command can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute(parameter);
        }

        /// <summary>
        /// Exectue the command Action
        /// </summary>
        /// <param name="parameter"></param>
        /// <exception cref="NotImplementedException"></exception>

        public void Execute(object? parameter)
        {
            _action(parameter);
        }

        // Method to manually trigger CanExecuteChanged event
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }


        #endregion
    }
}
