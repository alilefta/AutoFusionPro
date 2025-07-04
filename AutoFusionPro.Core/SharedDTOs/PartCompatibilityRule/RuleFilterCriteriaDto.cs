namespace AutoFusionPro.Core.SharedDTOs.PartCompatibilityRule
{
    public record RuleFilterCriteriaDto(
        string? SearchTerm = null, // Search Rule Name, Description, Part Number
        int? PartId = null,
        int? MakeId = null,
        int? ModelId = null,
        bool ShowTemplatesOnly = false,
        bool? ShowActiveOnly = null, // CHANGED to nullable bool. true=Active, false=Inactive, null=All

          // --- ADD THESE MISSING FIELDS ---
        int? TrimLevelId = null,
        int? EngineTypeId = null,
        int? TransmissionTypeId = null,
        int? BodyTypeId = null,
        int? ExactYear = null // Renamed from Min/Max to match your UI's "Exact Year" approach
    );

}
