using AutoFusionPro.Application.Commands;
using AutoFusionPro.Application.DTOs.User;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AutoFusionPro.UI.ViewModels.Settings.UserManagement
{
    public partial class UserManagementViewModel : BaseViewModel<UserManagementViewModel>
    {
        #region Private Fields

        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger<UserManagementViewModel> _logger;

        #endregion


        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private int _usersCount;


        [ObservableProperty]
        private ObservableCollection<UserDTO> _allUsers = new();




        public ICommand AddUserCommand { get; private set; }
        public ICommand DeleteUserCommand { get; private set; }
        public ICommand EditUserCommand { get; private set; }
        public ICommand RefreshDataCommand { get; private set; }

        public UserManagementViewModel(IUserService userService,
            ILocalizationService 
            localizationService,
            ILogger<UserManagementViewModel> logger) : base(localizationService, logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            #region Commands 

            AddUserCommand = new RelayCommand(action: o => ExecuteAddUserCommand(), canExecute: o => true);
            EditUserCommand = new RelayCommand(action: o => ExecuteEditUserCommand(o), canExecute: o => true);
            DeleteUserCommand = new RelayCommand(action: o => ExecuteDeleteUserCommand(o), canExecute: o => true);

            RefreshDataCommand = new RelayCommand(action: o => ExecuteRefreshDataCommand(), canExecute: o => true);

            #endregion
        }



        //private async Task LoadUserManagementData()
        //{
        //    try
        //    {
        //        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        //        {
        //            var users = await uow.Users.GetAllUsersAsync();
        //            AllUsers = new ObservableCollection<User>(users);

        //            UsersCount = users.Count();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("An error occured while loading user accounts for User Management");
        //        await MessageBoxHelper.ShowMessageWithoutTitleAsync(ex.Message, true, CurrentWorkFlow);
        //    }
        //}


        #region Command Methods


        private async void ExecuteRefreshDataCommand()
        {
            IsLoading = true;

            await Task.Delay(400);

            //await LoadUserManagementData();

            IsLoading = false;
        }

        private async void ExecuteAddUserCommand()
        {
            try
            {
                //var uow = _unitOfWorkFactory.CreateUnitOfWork();
                //var logger = _serviceProvider.GetRequiredService<ILogger<AddUserDialogViewModel>>();

                //var addUserDialog = new AddUserDialog();
                //var addUserVM = new AddUserDialogViewModel(_unitOfWorkFactory, _localizationService, logger, _authenticationService, _sessionManager, addUserDialog);

                //addUserDialog.Owner = System.Windows.Application.Current.MainWindow;
                //addUserDialog.DataContext = addUserVM;

                //bool? result = addUserDialog.ShowDialog();

                //if (result == true)
                //{
                //    // refresh data'
                //    await InitializeData();
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while adding a user, {ex}");

            }
        }



        private async void ExecuteEditUserCommand(object o)
        {
            if (o is UserDTO user)
            {
                try
                {
                    //var logger = _serviceProvider.GetRequiredService<ILogger<EditUserDialogViewModel>>();

                    //var editUserDialog = new EditUserDialog();
                    //var editUserVM = new EditUserDialogViewModel(_unitOfWorkFactory, _localizationService, logger, _authenticationService, _sessionManager, editUserDialog);

                    //editUserVM.InitializeAsync(user.Id);

                    //editUserDialog.Owner = System.Windows.Application.Current.MainWindow;
                    //editUserDialog.DataContext = editUserVM;

                    //bool? result = editUserDialog.ShowDialog();

                    //if (result == true)
                    //{

                    //    // refresh data'
                    //    await InitializeData();
                    //}




                }
                catch (Exception ex)
                {
                    _logger.LogError($"An error occured while editing a user {ex.ToString()}");

                }
            }
        }




        private async void ExecuteDeleteUserCommand(object o)
        {

            if (o is UserDTO user)
            {
                if (user == null) return;

                //var uow = _unitOfWorkFactory.CreateUnitOfWork();


                //// TODO: Check if last admin and Should promote new admin
                //if (user.Id == _sessionManager.CurrentUser.Id && user.IsAdmin)
                //{
                //    // 2. Check if this is the last admin user
                //    int adminCount = 0;
                //    try
                //    {
                //        var allUsers = await uow.Users.GetAllUsersAsync(); // Get all users to count admins
                //        adminCount = allUsers.Count(u => u.SystemRole == SystemRole.Admin);
                //    }
                //    catch (Exception ex)
                //    {
                //        _logger.LogError(ex, "Error counting admin users during self-delete check.");
                //        // Handle error gracefully - maybe assume not last admin to avoid blocking? Or show error and cancel?
                //        await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Admin Check Failed", "Could not verify admin users.", true, CurrentWorkFlow);
                //        return; // Exit deletion process
                //    }


                //    if (adminCount <= 1) // If only one admin left (or none after deletion) and deleting user is admin
                //    {

                //        var message = System.Windows.Application.Current.Resources["CannotRemoveLastAdmin"] as string ?? "You cannot delete the last administrator account. Please create another administrator or assign the Admin role to another user before deleting this account.";
                //        await MessageBoxHelper.ShowMessageWithTitleAsync(
                //            "Error",
                //            "Cannot Delete Last Admin",
                //            message,
                //            true,
                //            CurrentWorkFlow);
                //        return;
                //    }
                //}


                //var confirmDeleteDialog = new DeleteConfirmDialog();
                //confirmDeleteDialog.Owner = System.Windows.Application.Current.MainWindow;

                //var viewModel = new DeleteConfirmDialogViewModel
                //{
                //    CurrentWorkFlow = CurrentWorkFlow,
                //};
                //confirmDeleteDialog.Owner = System.Windows.Application.Current.MainWindow;
                //confirmDeleteDialog.DataContext = viewModel;



                //bool? result = confirmDeleteDialog.ShowDialog();

                //{
                //    try
                //    {

                //        // Remove current user
                //        if (user.Id == _sessionManager.CurrentUser.Id)
                //        {
                //            var message = Application.Current.Resources["RemoveCurrentlyLoggedInUser"] as string ?? "Do you want to remove the currently logged in account?";

                //            bool? res = await MessageBoxHelper.ShowConfirmMessageWithTitleAsync("Delete User", "Attention", message, false, "Yes", "No", Wpf.Ui.Controls.ControlAppearance.Caution, CurrentWorkFlow);

                //            if (res == true)
                //            {
                //                await uow.Users.DeleteUserAsync(user);

                //                _sessionManager.Logout();
                //                _navigationService.NavigateTo(ApplicationPage.Login);

                //            }
                //        }
                //        else
                //        {
                //            // Regular Remove
                //            await uow.Users.DeleteUserAsync(user);

                //            var message = Application.Current.Resources["SelectedUserDeletedStr"] as string ?? "Selected user have been deleted successfully.";

                //            await MessageBoxHelper.ShowMessageWithoutTitleAsync(message, false, CurrentWorkFlow);

                //            await LoadUserManagementData();
                //        }




                //    }
                //    catch (Exception ex)
                //    {
                //        // Handle exceptions
                //        _logger.LogError(ex, "Error deleting user with ID {Id}.", user.Id);
                //        await MessageBoxHelper.ShowMessageWithTitleAsync(title: "An error has occured", subtitle: "Deleting related error", content: $"An error occurred while deleting user: {ex.Message}", true, CurrentWorkFlow);

                //    }

                //}
            }
        }

        #endregion

    }
}
