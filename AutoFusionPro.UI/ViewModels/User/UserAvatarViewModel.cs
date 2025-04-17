using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Core.Services;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.Shell;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AutoFusionPro.UI.ViewModels.UserControls
{
    public class UserAvatarViewModel: BaseViewModel
    {
        //private IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILocalizationService<FlowDirection> _localizationService;
        private readonly ISessionManager _sessionManager;
        private readonly ILogger<UserAvatarViewModel> _logger;
        private readonly ShellViewModel _shellViewModel;
        private User _currentlyLoggedInUser;
        public User CurrentlyLoggedInUser
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


        public UserAvatarViewModel(ILocalizationService<FlowDirection>
            localizationService, ISessionManager sessionManager,
            IServiceProvider  serviceProvider,
            ILogger<UserAvatarViewModel> logger, ShellViewModel shellViewModel) 
        {

            //_unitOfWorkFactory = unitOfWorkFactory;
            _sessionManager = sessionManager;
            _logger = logger;
            _shellViewModel = shellViewModel;

            _localizationService = localizationService;
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
            _localizationService.FlowDirectionChanged += OnCurrentWorkFlowChanged;

            //ShowProfileCommand = new RelayCommand(action: o => ShowUserProfile(), canExecute: o => true);

            //ShowSettingsCommand = new RelayCommand(action: o => { ShowSettings(); }, canExecute: o => true);

            //LogoutCommand = new RelayCommand(action: o => { ShowLogoutConfirmDialog(); }, canExecute: o => _sessionManager.CurrentUser != null);

            _ = LoadUser();


        }

        private async Task LoadUser()
        {
            var user = _sessionManager.CurrentUser;
            if (user == null)
            {
                await MessageBoxHelper.ShowMessageWithoutTitleAsync("An error has occured with fetching User");

                _logger.LogError("No user is logged in.");
                return;

            }

            //var userID = _sessionManager.CurrentUser.Id;

            try
            {
                //using(var uow = _unitOfWorkFactory.CreateUnitOfWork())
                //{
                //    CurrentlyLoggedInUser = await uow.Users.GetUserByIdAsync(userID);
                //    if (CurrentlyLoggedInUser == null)
                //    {
                //        await MessageBoxHelper.ShowMessageWithoutTitleAsync("No User is logged in!", true, CurrentWorkFlow);
                //        return;
                //    }

                //    _ = LoadUserAvatar();
                //}
            }
            catch (Exception ex)
            {
                await MessageBoxHelper.ShowMessageWithTitleAsync("Showing user avatar","Error", $"An error has occured while trying to load User {ex.Message}", true, CurrentWorkFlow);

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
                    _logger.LogInformation($"Logo loaded from settings: {CurrentlyLoggedInUser.ProfilePictureUrl}");
                }
                else
                {
                    UserAvatar = new BitmapImage(new Uri("pack://application:,,,/OscarLab;component/Assets/Photos/Avatars/Avatar1.jpg"));
                    _logger.LogInformation("Profile Avatar Loaded");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading logo");
                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Error loading logo: {ex.Message}");
            }
        }
        //private void ShowLogoutConfirmDialog()
        //{
        //    _shellViewModel.RequestUserLogout();

        //}

        //private void ShowSettings()
        //{
        //    _shellViewModel.RequestShowSettings();
        //}

        //private void ShowUserProfile()
        //{
        //    _shellViewModel.RequestShowUserProfile();
        //}


        private void OnCurrentWorkFlowChanged()
        {
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
        }
    }
}
