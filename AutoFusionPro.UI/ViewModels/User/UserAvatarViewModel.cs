using AutoFusionPro.Application.Commands;
using AutoFusionPro.Application.DTOs.User;
using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.Shell;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AutoFusionPro.UI.ViewModels.User
{
    public class UserAvatarViewModel: BaseViewModel
    {
        //private IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ISessionManager _sessionManager;
        private readonly ILogger<UserAvatarViewModel> _logger;
        private readonly ShellViewModel _shellViewModel;
        private readonly IUserService _userService;

        private UserDTO _currentlyLoggedInUser;
        public UserDTO CurrentlyLoggedInUser
        {
            get => _currentlyLoggedInUser;
            set
            {
                _currentlyLoggedInUser = value;
                OnPropertyChanged(nameof(CurrentlyLoggedInUser));
            }
        }

        private BitmapImage _userAvatar;
        public BitmapImage UserAvatar

        {
            get => _userAvatar;
            set
            {
                _userAvatar = value;
                OnPropertyChanged(nameof(UserAvatar));
            }
        }

        public ICommand ShowProfileCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand LogoutCommand { get; }


        public UserAvatarViewModel(ILocalizationService
            localizationService, ISessionManager sessionManager,
            IServiceProvider  serviceProvider,
            ILogger<UserAvatarViewModel> logger, 
            ShellViewModel shellViewModel,
            IUserService userService) 
        {

            //_unitOfWorkFactory = unitOfWorkFactory;
            _sessionManager = sessionManager;
            _logger = logger;
            _shellViewModel = shellViewModel;
            _userService = userService;

            _localizationService = localizationService;
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
            _localizationService.FlowDirectionChanged += OnCurrentWorkFlowChanged;

            ShowProfileCommand = new RelayCommand(action: o => ShowUserProfile(), canExecute: o => true);

            ShowSettingsCommand = new RelayCommand(action: o => { ShowSettings(); }, canExecute: o => true);

            LogoutCommand = new RelayCommand(action: o => { ShowLogoutConfirmDialog(); }, canExecute: o => _sessionManager.CurrentUser != null);

            _ = LoadUser();


        }


        private async Task LoadUser()
        {

            if (!_sessionManager.Initialized.IsCompleted) return;

            var currentUserId = _sessionManager?.CurrentUser?.Id;

            if(currentUserId == null)
            {
                _logger.LogError("The user ID is null or not correct");
                return;
            }


            var userDTO =  _sessionManager?.CurrentUser;

            if (userDTO == null)
            {
                await MessageBoxHelper.ShowMessageWithoutTitleAsync("An error has occurred with fetching User");

                _logger.LogError("No user is logged in.");
                return;

            }

            //var userID = _sessionManager.CurrentUser.Id;

            try
            {
                //using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
                //{
                //    //CurrentlyLoggedInUser = await uow.Users.GetUserByIdAsync(userID);
                //    //if (CurrentlyLoggedInUser == null)
                //    //{
                //    //    await MessageBoxHelper.ShowMessageWithoutTitleAsync("No User is logged in!", true, CurrentWorkFlow);
                //    //    return;
                //    //}
                //}


                CurrentlyLoggedInUser = userDTO;

                _ = LoadUserAvatar();
            }
            catch (Exception ex)
            {
                await MessageBoxHelper.ShowMessageWithTitleAsync("Showing user avatar","Error", $"An error has occurred while trying to load User {ex.Message}", true, CurrentWorkFlow);
                throw new ApplicationException($"An error has occurred while trying to load User", ex);
            }
        }

        private async Task LoadUserAvatar()
        {
            await Task.Delay(50);

            try
            {
                if (!string.IsNullOrEmpty(CurrentlyLoggedInUser.ProfilePictureUrl) && File.Exists(CurrentlyLoggedInUser.ProfilePictureUrl))
                {
                    var bitmap = new BitmapImage(new Uri(CurrentlyLoggedInUser.ProfilePictureUrl, UriKind.Absolute));
                    bitmap.Freeze();
                    UserAvatar = bitmap;
                    _logger.LogInformation($"User Avatar loaded from settings: {CurrentlyLoggedInUser.ProfilePictureUrl}");
                }
                else
                {
                    UserAvatar = new BitmapImage(new Uri("pack://application:,,,/AutoFusionPro.UI;component/Assets/Photos/Avatars/Avatar1.jpg"));
                    _logger.LogInformation("Default Profile Avatar Loaded");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading logo");
                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Error loading logo: {ex.Message}");
            }
        }
        private void ShowLogoutConfirmDialog()
        {
            _shellViewModel.RequestUserLogout();

        }

        private void ShowSettings()
        {
            _shellViewModel.RequestShowSettings();
        }

        private void ShowUserProfile()
        {
            _shellViewModel.RequestShowUserProfile();
        }


        private void OnCurrentWorkFlowChanged()
        {
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
        }
    }
}
