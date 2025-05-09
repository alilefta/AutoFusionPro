using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.UI.Extensions;
using AutoFusionPro.UI.Services.Animations;
using System.Windows;

namespace AutoFusionPro.UI.Services.Dialogs
{
    /// <summary>
    /// Dialog adapter that supports animations
    /// </summary>
    /// 

    public class AnimatedDialogWindowAdapter : IDialogWindow
    {
        private readonly Window _window;
        private readonly AnimationHelpers.AnimationType _showAnimationType;
        private readonly AnimationHelpers.AnimationType _closeAnimationType;
        private bool _isAnimatingClose = false;

        public AnimatedDialogWindowAdapter(
            Window window,
            AnimationHelpers.AnimationType showAnimationType = AnimationHelpers.AnimationType.FadeIn,
            AnimationHelpers.AnimationType closeAnimationType = AnimationHelpers.AnimationType.FadeOut)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _showAnimationType = showAnimationType;
            _closeAnimationType = closeAnimationType;
        }

        public bool? DialogResult
        {
            get => _window.DialogResult;
            set => _window.DialogResult = value;
        }

        public void Close()
        {
            // Prevent multiple close animations
            if (_isAnimatingClose)
                return;

            _isAnimatingClose = true;
            _window.CloseWithAnimation(_closeAnimationType);
        }
    }
}
