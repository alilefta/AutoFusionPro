using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using System.Windows;

namespace AutoFusionPro.Application.Interfaces.Dialogs
{
    public interface IDialogService
    {
        /// <summary>
        /// Show Add Vehicle Dialog
        /// </summary>
        /// <returns></returns>
        bool? ShowAddVehicleDialog();

        #region Add Compatible Vehicles Dialogs

        /// <summary>
        /// Shows Add Make Dialog
        /// </summary>
        /// <returns><see cref="bool"/> of dialog results</returns>
        bool? ShowAddMakeDialog();

        // Task because it is a ViewModel with initialization
        /// <summary>
        /// Shows Add Model Dialog 
        /// </summary>
        /// <param name="makeId">Models need a <see cref="DTOs.CompatibleVehicleDTOs.MakeDto.Id"/> to be assigned to.</param>
        /// <returns><see cref="bool"/> of dialog results</returns>
        Task<bool?> ShowAddModelDialog(int makeId);

        /// <summary>
        /// Shows Add Trim Level Dialog 
        /// </summary>
        /// <param name="modelId">Trim Level needs <see cref="DTOs.CompatibleVehicleDTOs.ModelDto.Id"/> to be assigned to.</param>
        /// <returns><see cref="bool"/> of dialog results</returns>
        Task<bool?> ShowAddTrimLevelDialog(int modelId);


        #endregion

        #region Edit Compatible Vehicles Dialogs

        /// <summary>
        /// Shows Edit a Make Dialog
        /// </summary>
        /// <param name="makeDto">Makes need to track its ID to be updated.</param>
        /// <returns><see cref="bool"/> of dialog results</returns>
        Task<bool?> ShowEditMakeDialog(MakeDto makeDto);

        // Task because it is a ViewModel with initialization
        /// <summary>
        /// Shows Edit a Model Dialog 
        /// </summary>
        /// <param name="updateModelDto">Models need a <see cref="MakeDto.Id"/> to be assigned to.</param>
        /// <returns><see cref="bool"/> of dialog results</returns>
        Task<bool?> ShowEditModelDialog(ModelDto updateModelDto);

        /// <summary>
        /// Shows Edit a Trim Level Dialog 
        /// </summary>
        /// <param name="updateTrimLevelDto">Trim Level needs <see cref="ModelDto.Id"/> to be assigned to.</param>
        /// <returns><see cref="bool"/> of dialog results</returns>
        Task<bool?> ShowEditTrimLevelDialog(TrimLevelDto updateTrimLevelDto);


        #endregion
        /// <summary>
        /// General Delete Confirm Dialog
        /// </summary>
        /// <param name="count">Number of items to be deleted, just for UI show</param>
        /// <returns></returns>
        bool? ShowConfirmDeleteItemsDialog(int count);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDialogViewModel"></typeparam>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TParam"></typeparam>
        /// <param name="configureViewModel"></param>
        /// <returns></returns>
        Task<bool?> ShowDialogAsync<TDialogViewModel, TView>(object? param = null) where TDialogViewModel : class where TView : Window, new();
    }



    //For future scalability if you end up with 20+ dialog types, you could consider a more generic approach on IDialogService:
    //public interface IDialogService
    //{
    //    Task<TResult?> ShowDialogAsync<TDialogViewModel, TView, TResult>(Action<TDialogViewModel>? configureViewModel = null)
    //        where TDialogViewModel : class, IDialogViewModel<TResult> // IDialogViewModel returns TResult
    //        where TView : Window, new(); // Or some base dialog window type
    //                                     // ...
    //}
}
