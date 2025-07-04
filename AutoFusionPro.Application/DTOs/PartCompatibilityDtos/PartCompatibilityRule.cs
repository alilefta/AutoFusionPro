using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.PartCompatibilityDtos
{
    /// <summary>
    /// Represents a specific attribute (like a TrimLevel, EngineType) linked to a compatibility rule,
    /// including whether it's an inclusion or exclusion.
    /// </summary>
    public record PartCompatibilityRuleApplicableAttributeDto(
        int AttributeId,        // e.g., TrimLevelId, EngineTypeId
        string AttributeName,    // e.g., "SV", "2.5L I4" (for display in rule summary)
        bool IsExclusion = false
    );

    // You might also have specific versions if needed, e.g.:
    // public record PartCompatibilityRuleApplicableTrimDto(int TrimLevelId, string TrimLevelName, bool IsExclusion);
    // But a generic one can work if AttributeType is implied by the collection it's in.



    /// <summary>
    /// DTO for creating a new Part Compatibility Rule.
    /// PartId will be a separate parameter in the service method.
    /// </summary>
    public record CreatePartCompatibilityRuleDto(
        [Required(AllowEmptyStrings = false)]
        [StringLength(150, ErrorMessage = "Rule name cannot exceed 150 characters.")]
        string Name, // User-defined name for the rule

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        string? Description,

        // Core Filters (IDs from your taxonomy DTOs like MakeDto, ModelDto)
        int? MakeId,    // Null means rule applies to any Make (if ModelId is also null)
        int? ModelId,   // Null means rule applies to any Model (within selected Make if MakeId is set)

        // Year Range
        // Validation: If one is set, the other should ideally be set too, or YearEnd >= YearStart
        int? YearStart,
        int? YearEnd,

        // Applicable Attributes (if null or empty, means "ANY" for that attribute type)
        // The list contains the IDs of the selected taxonomy items (e.g., TrimLevel IDs)
        List<PartCompatibilityRuleApplicableAttributeDto>? ApplicableTrimLevels,
        List<PartCompatibilityRuleApplicableAttributeDto>? ApplicableEngineTypes,
        List<PartCompatibilityRuleApplicableAttributeDto>? ApplicableTransmissionTypes,
        List<PartCompatibilityRuleApplicableAttributeDto>? ApplicableBodyTypes,

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        string? Notes, // Notes specific to this rule's application to the part

        bool IsActive = true,

        // For template functionality later
        bool CreateAsTemplate = false,
        int? CopiedFromRuleId = null // If this rule is a copy
    );



    /// <summary>
    /// DTO for updating an existing Part Compatibility Rule.
    /// </summary>
    public record UpdatePartCompatibilityRuleDto(
        [Range(1, int.MaxValue)]
        int Id, // ID of the PartCompatibilityRule to update

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, ErrorMessage = "Rule name cannot exceed 150 characters.")]
        string Name,

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        string? Description,

        int? MakeId,
        int? ModelId,
        int? YearStart,
        int? YearEnd,

        // For updates, you typically send the complete new list of applicable attributes.
        // The service layer will handle diffing or replacing the existing linked attributes.
        List<PartCompatibilityRuleApplicableAttributeDto>? ApplicableTrimLevels,
        List<PartCompatibilityRuleApplicableAttributeDto>? ApplicableEngineTypes,
        List<PartCompatibilityRuleApplicableAttributeDto>? ApplicableTransmissionTypes,
        List<PartCompatibilityRuleApplicableAttributeDto>? ApplicableBodyTypes,

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        string? Notes,

        bool IsActive,
        bool IsTemplate // Can an existing rule be marked as a template?
    );




    /// <summary>
    /// DTO for displaying a summary of a Part Compatibility Rule.
    /// </summary>
    public record PartCompatibilityRuleSummaryDto(
        int Id,
        string Name,
        string? Description,
        string MakeNameDisplay,         // "Any Make" or actual Make name
        string ModelNameDisplay,        // "Any Model" or actual Model name
        string YearRangeDisplay,        // "Any Year", "2019-2022", "2020 Only", "Up to 2021", "From 2020"
        string TrimsDisplay,            // "Any Trim", "S, SV (Excludes: SR)", "Only: S, SV"
        string EnginesDisplay,          // Similar to TrimsDisplay
        string TransmissionsDisplay,    // Similar
        string BodyTypesDisplay,        // Similar
        bool IsActive,
        bool IsTemplate,
        DateTime CreatedAt,
        DateTime? ModifiedAt,
        int PartId, // To know which part it's for if viewing a global list of rules
        string PartNumber // For context
    );
    // The "Display" strings will be constructed by the service layer based on the rule's settings.



    public class PartCompatibilityRuleDetailDto // Class for potential INPC if needed
    {
        public int Id { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; } = string.Empty; // For context
        public string PartNumber { get; set; } = string.Empty; // For context

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public int? MakeId { get; set; }
        public string? MakeName { get; set; } // For display

        public int? ModelId { get; set; }
        public string? ModelName { get; set; } // For display

        public int? YearStart { get; set; }
        public int? YearEnd { get; set; }

        // Collections of selected/excluded attributes with their details
        public List<PartCompatibilityRuleApplicableAttributeDto> ApplicableTrimLevels { get; set; } = new();
        public List<PartCompatibilityRuleApplicableAttributeDto> ApplicableEngineTypes { get; set; } = new();
        public List<PartCompatibilityRuleApplicableAttributeDto> ApplicableTransmissionTypes { get; set; } = new();
        public List<PartCompatibilityRuleApplicableAttributeDto> ApplicableBodyTypes { get; set; } = new();

        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public bool IsTemplate { get; set; }
        public int? CopiedFromRuleId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }



    // Supporting DTOs for the query service aspect (might live in a shared DTOs namespace or PartCompatibilityRule DTOs)
    // This DTO describes a specific vehicle a user is interested in, to find parts for.
    // It's DIFFERENT from the old CompatibleVehicleDto which WAS a spec in the DB.
    // This is an INPUT DTO for querying.
    public record CompatibilityMatchResultDto(
        bool IsCompatible,
        string MatchReason, // e.g., "Matches Rule: Nissan Sedans 2020+"
        List<string> AppliedRuleNames // Names of the rules that caused the match
    );

    public record ShowRulesByActivityFilterDto(string title, bool? value);

    public record VehicleSpecificationDto( // Renamed to avoid confusion with old CompatibleVehicle entity DTOs
        int? MakeId,
        int? ModelId,
        int? TrimLevelId,
        int? TransmissionTypeId,
        int? EngineTypeId,
        int? BodyTypeId,
        int Year // A specific year for matching
    );



    // New DTO for filtering rules in FindPartIdsMatchingVehicleSpecAsync
    // This DTO can include criteria that apply to the PartCompatibilityRule itself
    // or to the Part it's linked to.
    public record PartCompatibilityRuleFilterDto(
        bool OnlyActiveRules = true,
        bool OnlyActiveParts = true, // Filter by the Part's IsActive status
        int? PartCategoryId = null   // Filter by the Part's CategoryId
                                     // Add other relevant filters that can be applied at the rule/part level before evaluating compatibility
    );

    public record AttributeMatchItem(int Id, bool IsExclusion);
}
