using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs
{
    #region Compatible Vehicle DTOs
    public record CompatibleVehicleSummaryDto(
        int Id,
        string MakeName,
        string ModelName,
        int YearStart,
        int YearEnd,
        string? TrimLevelName,
        string? TransmissionTypeName,
        string? EngineTypeName,
        string? BodyTypeName,
        string? VIN // Included VIN for summary display if present
    );


    /// <summary>
    /// DTO for displaying and editing the full details of a CompatibleVehicle configuration.
    /// Inherits ObservableObject if direct two-way binding from a form to this DTO is planned.
    /// Otherwise, a plain class is fine and the ViewModel manages individual observable properties.
    /// </summary>
    public partial class CompatibleVehicleDetailDto : ObservableObject // Or just 'public class ...'
    {
        // Use [ObservableProperty] if inheriting ObservableObject for direct binding
        // Otherwise, use standard get; set;

        [ObservableProperty] private int _id;
        [ObservableProperty] private int _modelId;
        [ObservableProperty] private string _makeName = string.Empty;
        [ObservableProperty] private string _modelName = string.Empty;
        [ObservableProperty] private int _yearStart;
        [ObservableProperty] private int _yearEnd;
        [ObservableProperty] private int? _trimLevelId;
        [ObservableProperty] private string? _trimLevelName;
        [ObservableProperty] private int? _transmissionTypeId;
        [ObservableProperty] private string? _transmissionTypeName;
        [ObservableProperty] private int? _engineTypeId;
        [ObservableProperty] private string? _engineTypeName;
        [ObservableProperty] private int? _bodyTypeId;
        [ObservableProperty] private string? _bodyTypeName;
        [ObservableProperty] private string? _vin;

        // Parameterless constructor for frameworks/deserialization/ObservableObject
        public CompatibleVehicleDetailDto() { }

        // Optional: Constructor for manual mapping
        public CompatibleVehicleDetailDto(
            int id, int modelId, string makeName, string modelName, int yearStart, int yearEnd,
            int? trimLevelId, string? trimLevelName,
            int? transmissionTypeId, string? transmissionTypeName,
            int? engineTypeId, string? engineTypeName,
            int? bodyTypeId, string? bodyTypeName,
            string? vin)
        {
            Id = id;
            ModelId = modelId;
            MakeName = makeName;
            ModelName = modelName;
            YearStart = yearStart;
            YearEnd = yearEnd;
            TrimLevelId = trimLevelId;
            TrimLevelName = trimLevelName;
            TransmissionTypeId = transmissionTypeId;
            TransmissionTypeName = transmissionTypeName;
            EngineTypeId = engineTypeId;
            EngineTypeName = engineTypeName;
            BodyTypeId = bodyTypeId;
            BodyTypeName = bodyTypeName;
            Vin = vin;
        }
    }

    public record CreateCompatibleVehicleDto(
        [Range(1, int.MaxValue, ErrorMessage = "Model must be selected.")] int ModelId,
        [Range(1900, 2100, ErrorMessage = "Start year must be valid.")] int YearStart,
        [Range(1900, 2100, ErrorMessage = "End year must be valid.")] int YearEnd,
        int? TrimLevelId,
        int? TransmissionTypeId,
        int? EngineTypeId,
        int? BodyTypeId,
        [StringLength(20, ErrorMessage = "VIN cannot exceed 20 characters.")] string? VIN
    );


    // FluentValidation Rules (in service or validator class):
    // - YearEnd >= YearStart
    // - Uniqueness check for the whole spec

    public record UpdateCompatibleVehicleDto(
       [Range(1, int.MaxValue)] int Id,
       [Range(1, int.MaxValue, ErrorMessage = "Model must be selected.")] int ModelId,
       [Range(1900, 2100, ErrorMessage = "Start year must be valid.")] int YearStart,
       [Range(1900, 2100, ErrorMessage = "End year must be valid.")] int YearEnd,
       int? TrimLevelId,
       int? TransmissionTypeId,
       int? EngineTypeId,
       int? BodyTypeId,
       [StringLength(20, ErrorMessage = "VIN cannot exceed 20 characters.")] string? VIN
   );
    // FluentValidation Rules (in service or validator class):
    // - YearEnd >= YearStart
    // - Uniqueness check for the whole spec (excluding self)


    public record CompatibleVehicleFilterCriteriaDto(
       string? SearchTerm = null,
       int? MakeId = null,
       int? ModelId = null,
       int? ExactYear = null, // To find specs where YearStart <= ExactYear <= YearEnd
       int? TrimLevelId = null,
       int? TransmissionTypeId = null,
       int? EngineTypeId = null,
       int? BodyTypeId = null
   );


    #endregion

    #region Make DTOs

    public record MakeDto(int Id, string Name, string? ImagePath);
    public record CreateMakeDto(string Name, string? ImagePath); // Validation: Name required, unique
    public record UpdateMakeDto(int Id, string Name, string? ImagePath); // Validation: Name required, unique (excluding self)

    #endregion

    #region Model DTOs

    public record ModelDto(int Id, string Name, int MakeId, string MakeName, string? ImagePath); // MakeName for display context
    public record CreateModelDto(string Name, int MakeId, string? ImagePath); // Validation: Name & MakeId required, Name unique within Make
    public record UpdateModelDto(int Id, string Name, int MakeId, string? ImagePath); // Validation: Name & MakeId required, Name unique within Make (excluding self)

    #endregion

    #region Trim Level DTOs

    public record TrimLevelDto(int Id, string Name, int ModelId, string ModelName); // ModelName for display context
    public record CreateTrimLevelDto(string Name, int ModelId); // Validation: Name & ModelId required, Name unique within Model
    public record UpdateTrimLevelDto(int Id, string Name, int ModelId); // Validation: Name & ModelId required, Name unique within Model (excluding self)

    #endregion

    #region Transmission Type DTOs

    public record TransmissionTypeDto(int Id, string Name);
    // Using generic lookup DTOs for create/update if only 'Name' is involved
    // public record CreateTransmissionTypeDto(string Name);
    // public record UpdateTransmissionTypeDto(int Id, string Name);

    #endregion

    #region Engine Type DTOs

    public record EngineTypeDto(int Id, string Name, string? Code);
    public record CreateEngineTypeDto(string Name, string? Code); // Validation: Name required, Name/Code unique
    public record UpdateEngineTypeDto(int Id, string Name, string? Code); // Validation: Name required, Name/Code unique (excluding self)

    #endregion

    #region Body Type DTOs

    public record BodyTypeDto(int Id, string Name);
    // Using generic lookup DTOs for create/update if only 'Name' is involved
    // public record CreateBodyTypeDto(string Name);
    // public record UpdateBodyTypeDto(int Id, string Name);

    #endregion

    #region General Lookup DTOs

    /// <summary>
    /// Generic DTO for creating simple lookup entities that only have a Name.
    /// </summary>
    public record CreateLookupDto(
        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 2)]
        string Name
    );

    /// <summary>
    /// Generic DTO for updating simple lookup entities that only have a Name.
    /// </summary>
    public record UpdateLookupDto(
        [Range(1, int.MaxValue)]
        int Id,
        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 2)]
        string Name
    );

    #endregion

    #region Filtration 

    public partial class ActiveFilterDisplayItem : ObservableObject
    {
        [ObservableProperty]
        private string _filterType; // e.g., "Make", "Model", "Year"

        [ObservableProperty]
        private string _filterValueDisplay; // e.g., "Nissan", "Altima", "2020"

        public object OriginalCriterion { get; } // To know what to remove (e.g., the MakeId)
        public string CriterionKey { get; } // e.g., "MakeId", "ModelId"

        public ActiveFilterDisplayItem(string filterType, string filterValueDisplay, string criterionKey, object originalCriterion)
        {
            _filterType = filterType;
            _filterValueDisplay = filterValueDisplay;
            CriterionKey = criterionKey;
            OriginalCriterion = originalCriterion;
        }
    }

    /// <summary>
    /// Used for Populating Years collection AND allow for a Null value with text like "All Years" which matters for Filtration.
    /// </summary>
    /// <param name="Year">Year Number</param>
    /// <param name="DisplayName">String Representation of the Year</param>
    public record YearFilterItem(int? Year, string DisplayName);

    #endregion
}
