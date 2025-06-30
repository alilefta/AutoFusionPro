using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.PartImage
{
    public record PartImageDto(
           int Id,
           string ImagePath,
           string? Caption,
           bool IsPrimary,
           int DisplayOrder
       );

    public partial class CreatePartImageDto : ObservableValidator // Make it partial
    {
        [ObservableProperty] // This now represents the SOURCE CLIENT PATH
        private string _imagePath = string.Empty;

        [ObservableProperty]
        [StringLength(255)]
        private string? _caption;

        [ObservableProperty]
        private bool _isPrimary = false;

        [ObservableProperty]
        [Range(0, int.MaxValue)]
        private int _displayOrder = 0;

        public CreatePartImageDto(string imagePath)
        {
            ImagePath = imagePath;
        }

        public CreatePartImageDto(string imagePath, string? caption, bool? isPrimary, int? displayOrder)
        {
            ImagePath = imagePath; // This is the local path of the file just selected by user
            Caption = caption ?? string.Empty;
            IsPrimary = isPrimary ?? false;
            DisplayOrder = displayOrder ?? 0;
        }
    }

    public record UpdatePartImageDto(
       [Range(1, int.MaxValue)]
        int Id, // ID of the PartImage record
       [StringLength(255)]
        string? Caption,
       bool IsPrimary,
       [Range(0, int.MaxValue)]
        int DisplayOrder,
        string ImagePath

   );


    public partial class PartImageDisplayWrapper : ObservableObject
    {
        public int ExistingPartImageId { get; } // 0 if new
        [ObservableProperty] private string _imagePathForDisplay; // Source path for new, persistent for existing
        [ObservableProperty] private string? _caption;
        [ObservableProperty] private bool _isPrimary;
        [ObservableProperty] private int _displayOrder;
        public bool IsNew => ExistingPartImageId == 0;

        public PartImageDisplayWrapper(string path, int existingId = 0)
        { 
            ImagePathForDisplay = path; 
            ExistingPartImageId = existingId; 
        }
    }

    public record CreatePartImageInitialDto(string SourceClientPath, string? Caption, bool IsPrimary, int DisplayOrder);

}
