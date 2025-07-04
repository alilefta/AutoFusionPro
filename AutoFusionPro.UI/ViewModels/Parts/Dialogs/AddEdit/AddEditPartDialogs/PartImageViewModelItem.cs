using AutoFusionPro.Application.DTOs.PartImage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoFusionPro.UI.ViewModels.Parts.Dialogs.AddEdit.AddEditPartDialogs
{
    public enum ImageItemState { Unchanged, Added, Modified, Removed }

    public partial class PartImageViewModelItem : ObservableValidator
    {

        private AddEditPartDialogViewModel _parentViewModel { get; set; }

        // If it's an existing image from the DB
        public int ExistingPartImageId { get; }
        public string? PersistentImagePath { get; } // Stored path for existing images

        // For newly added images from local disk
        public string? SourceClientPath { get; } // Local path for a new image

        [ObservableProperty]
        private string? _imagePathForPreview; // Path used by UI for <Image Source>
                                              // For new images: SourceClientPath
                                              // For existing images: PersistentImagePath

        [ObservableProperty]
        [System.ComponentModel.DataAnnotations.StringLength(255)] // Example validation
        private string? _caption;

        [ObservableProperty]
        private bool _isPrimary;

        [ObservableProperty]
        [System.ComponentModel.DataAnnotations.Range(0, int.MaxValue)]
        private int _displayOrder;

        [ObservableProperty]
        private ImageItemState _state;

        public bool IsNew => ExistingPartImageId == 0;

        // Constructor for a NEW image selected by user
        public PartImageViewModelItem(string sourceClientPath, AddEditPartDialogViewModel parentViewModel)
        {
            SourceClientPath = sourceClientPath;
            ImagePathForPreview = sourceClientPath; // Preview the local file
            State = ImageItemState.Added;
            ExistingPartImageId = 0;

            _parentViewModel = parentViewModel;

        }

        // Constructor for an EXISTING image loaded from DB
        public PartImageViewModelItem(PartImageDto existingImageDto, AddEditPartDialogViewModel parentViewModel)
        {
            ExistingPartImageId = existingImageDto.Id;
            PersistentImagePath = existingImageDto.ImagePath;
            ImagePathForPreview = existingImageDto.ImagePath; // Preview the stored image
            Caption = existingImageDto.Caption;
            IsPrimary = existingImageDto.IsPrimary;
            DisplayOrder = existingImageDto.DisplayOrder;
            State = ImageItemState.Unchanged;


            _parentViewModel = parentViewModel;
        }

        // When a property changes on an existing item, mark it as Modified
        partial void OnCaptionChanged(string? value) => UpdateStateIfExisting();
        partial void OnIsPrimaryChanged(bool value)
        {
            UpdateStateIfExisting();
            // ViewModel needs to handle unchecking other primary images
        }
        partial void OnDisplayOrderChanged(int value) => UpdateStateIfExisting();

        private void UpdateStateIfExisting()
        {
            if (!IsNew && State == ImageItemState.Unchanged)
            {
                State = ImageItemState.Modified;
            }
        }

        partial void OnIsPrimaryChanged(bool oldValue, bool newValue)
        {
            if (newValue) // If this image is being set to primary
            {
                // Notify the parent AddEditPartDialogViewModel to handle unchecking others
                // This requires a way for the item to communicate back to the parent VM.
                // Option 1: Pass parent VM to item constructor.
                // Option 2: Use a messaging system (e.g., MVVM Toolkit Messenger).
                // Option 3: Handle in AddEditPartDialogViewModel by observing the collection.

                // Let's use Option 3 for now (simpler event handler or CollectionChanged logic in AddEditPartDialogViewModel)
                // Alternatively, the main VM can subscribe to PropertyChanged of each item.
                // For now, let's assume the main ViewModel will iterate after a primary is set by UI.
                _parentViewModel?.HandlePrimaryImageSet(this); // Assuming ParentViewModel reference exists
            }
            if (!IsNew && State == ImageItemState.Unchanged) State = ImageItemState.Modified;
        }


        [RelayCommand]
        private void SetAsPrimary(PartImageViewModelItem imageItem)
        {
            if (imageItem is not null)
            {
                imageItem.IsPrimary = true;
            }
        }



    }
}
