namespace AutoFusionPro.UI.Helpers.Filtration
{
    /// <summary>
    /// Used to solve the problem of 2 state boolean (like filtering by HasSubcategories with checkbox, you can Filter if yes, or no, but you can't remove the filter!).
    /// This record will populate a combobox with 3 values
    /// </summary>
    /// <param name="DisplayName"></param>
    /// <param name="Value"></param>
    public record BooleanFilterOption(
           string DisplayName, // e.g., "Any", "Yes", "No"
           bool? Value         // null for "Any", true for "Yes", false for "No"
       );
}
