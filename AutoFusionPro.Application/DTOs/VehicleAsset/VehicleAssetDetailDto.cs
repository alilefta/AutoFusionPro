using AutoFusionPro.Core.Enums.ModelEnum.VehicleInventory;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoFusionPro.Application.DTOs.VehicleAsset
{
    // DTOs for child collections (assuming they are defined or will be defined)
    // These might live in their own files or a shared DTO file.
    public record VehicleImageDto(int Id, string ImagePath, string? Caption, bool IsPrimary, int DisplayOrder);
    public record VehicleDamageImageDto(int Id, string ImagePath, string? Caption); // Simplified for this example
    public record VehicleDamageLogDto(int Id, DateTime DateNoted, string Description, DamageSeverity Severity, string SeverityDisplay, bool IsRepaired, DateTime? RepairedDate, decimal? EstimatedRepairCost, decimal? ActualRepairCost, string? RepairNotes, List<VehicleDamageImageDto> DamageImages);
    public record VehicleServiceRecordDto(int Id, DateTime ServiceDate, int? MileageAtService, string ServiceDescription, string? ServiceProviderName, decimal? Cost, string? Notes);
    public record VehicleDocumentDto(int Id, string DocumentName, DocumentType DocumentType, string DocumentTypeDisplay, string FilePath, DateTime? ExpiryDate);

    /// <summary>
    /// DTO for displaying and potentially editing the full details of a Vehicle asset.
    /// </summary>
    public partial class VehicleAssetDetailDto : ObservableObject // Or just 'public class ...'
    {
        // Using [ObservableProperty] for brevity if this class directly supports two-way binding in a form
        // If not, these would be standard auto-properties (get; set;)

        [ObservableProperty] private int _id;

        // Specification IDs & Names
        [ObservableProperty] private int _makeId;
        [ObservableProperty] private string _makeName = string.Empty;
        [ObservableProperty] private int _modelId;
        [ObservableProperty] private string _modelName = string.Empty;
        [ObservableProperty] private int _year;
        [ObservableProperty] private int? _engineTypeId;
        [ObservableProperty] private string? _engineTypeName;
        [ObservableProperty] private int? _transmissionTypeId;
        [ObservableProperty] private string? _transmissionTypeName;
        [ObservableProperty] private int? _trimLevelId;
        [ObservableProperty] private string? _trimLevelName;
        [ObservableProperty] private int? _bodyTypeId;
        [ObservableProperty] private string? _bodyTypeName;

        // Identification
        [ObservableProperty] private string? _vin;
        [ObservableProperty] private string? _registrationPlateNumber;
        [ObservableProperty] private DateTime? _registrationExpiryDate;
        [ObservableProperty] private string? _registrationCountryOrState;

        // Condition & Appearance
        [ObservableProperty] private string? _exteriorColor;
        [ObservableProperty] private string? _interiorColor;
        [ObservableProperty] private int? _mileage;
        [ObservableProperty] private MileageUnit? _mileageUnit;
        [ObservableProperty] private string? _mileageUnitDisplay;

        // Mechanicals
        [ObservableProperty] private FuelType? _fuelType;
        [ObservableProperty] private string? _fuelTypeDisplay;
        [ObservableProperty] private DriveType? _driveType;
        [ObservableProperty] private string? _driveTypeDisplay;
        [ObservableProperty] private int? _numberOfDoors;
        [ObservableProperty] private int? _numberOfSeats;

        // Sales & Acquisition
        [ObservableProperty] private VehicleStatus _status;
        [ObservableProperty] private string _statusDisplay = string.Empty;
        [ObservableProperty] private decimal? _purchasePrice;
        [ObservableProperty] private DateTime? _purchaseDate;
        [ObservableProperty] private decimal? _askingPrice;
        [ObservableProperty] private decimal? _soldPrice;
        [ObservableProperty] private DateTime? _soldDate;
        [ObservableProperty] private int? _soldToCustomerId;
        [ObservableProperty] private string? _soldToCustomerName; // For display

        // Notes & Features
        [ObservableProperty] private string? _generalNotes;
        [ObservableProperty] private string? _featuresList;

        // Timestamps
        [ObservableProperty] private DateTime _createdAt;
        [ObservableProperty] private DateTime? _modifiedAt;

        // Collections of related details
        [ObservableProperty] private List<VehicleImageDto> _images = new();
        [ObservableProperty] private List<VehicleDamageLogDto> _damageLogs = new();
        [ObservableProperty] private List<VehicleServiceRecordDto> _serviceRecords = new();
        [ObservableProperty] private List<VehicleDocumentDto> _documents = new();

        public VehicleAssetDetailDto() { } // Needed for ObservableObject or deserialization
    }
}
