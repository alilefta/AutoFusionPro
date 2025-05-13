namespace AutoFusionPro.Application.Interfaces.Dialogs
{
    public interface IDialogService
    {
        bool? ShowAddVehicleDialog();

        #region  Compatibility Dialogs

        bool? ShowAddMakeDialog();

        // Task because it is a ViewModel with initialization
        Task<bool?> ShowAddModelDialog(int makeId);
        Task<bool?> ShowAddTrimLevelDialog(int modelId);

        #endregion
    }
}
