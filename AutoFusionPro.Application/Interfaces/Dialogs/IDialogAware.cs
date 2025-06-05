namespace AutoFusionPro.Application.Interfaces.Dialogs
{
    /// <summary>
    /// Sets the specific Dialog Window <see cref="IDialogWindow"/> to its inheritance view model classes. 
    /// </summary>
    public interface IDialogAware
    {
        /// <summary>
        /// The <see cref="IDialogWindow"/> is injected via <see cref="IDialogService"/> methods to allow DialogViewModels to set results and return results of the <see cref="IDialogWindow"/> for the <see cref="IDialogService"/>
        /// </summary>
        /// <param name="dialog"></param>
        void SetDialogWindow(IDialogWindow dialog);
    }
}
