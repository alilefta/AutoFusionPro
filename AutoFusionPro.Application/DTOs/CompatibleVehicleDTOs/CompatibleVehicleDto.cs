using AutoFusionPro.Application.DTOs.Base;
using AutoFusionPro.Application.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs
{

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

    public record TrimLevelDto(int Id, string Name, int ModelId, string ModelName) : ISelectableDto; // ModelName for display context
    public record CreateTrimLevelDto(string Name, int ModelId) ; // Validation: Name & ModelId required, Name unique within Model
    public record UpdateTrimLevelDto(int Id, string Name, int ModelId); // Validation: Name & ModelId required, Name unique within Model (excluding self)

    #endregion

    #region Transmission Type DTOs

    public record TransmissionTypeDto(int Id, string Name) : ISelectableDto;
    // Using generic lookup DTOs for create/update if only 'Name' is involved
    // public record CreateTransmissionTypeDto(string Name);
    // public record UpdateTransmissionTypeDto(int Id, string Name);

    #endregion

    #region Engine Type DTOs

    public record EngineTypeDto(int Id, string Name, string? Code) : ISelectableDto;
    public record CreateEngineTypeDto(string Name, string? Code); // Validation: Name required, Name/Code unique
    public record UpdateEngineTypeDto(int Id, string Name, string? Code); // Validation: Name required, Name/Code unique (excluding self)

    #endregion

    #region Body Type DTOs

    public record BodyTypeDto(int Id, string Name) : ISelectableDto;
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
    /// <summary>
    /// Used for Populating Years collection AND allow for a Null value with text like "All Years" which matters for Filtration.
    /// </summary>
    /// <param name="Year">Year Number</param>
    /// <param name="DisplayName">String Representation of the Year</param>
    public record YearFilterItem(int? Year, string DisplayName);

    #endregion

    #region Selection 

    /// <summary>
    /// Wrapper used for selecting multiple trim level items for Adding Part Compatibility
    /// </summary>
    public partial class SelectableItemWrapper<TDtoType> : ObservableObject
        where TDtoType : class, ISelectableDto // Add constraint
    {
        private readonly IMessenger _messenger;

        [ObservableProperty]
        private TDtoType? _dtoItem;

        [ObservableProperty]
        private bool _isSelected;

        // The wrapper needs access to the messenger
        public SelectableItemWrapper(TDtoType? dto, bool isSelected, IMessenger messenger)
        {
            _dtoItem = dto;
            _isSelected = isSelected;
            _messenger = messenger;
        }

        partial void OnIsSelectedChanged(bool value)
        {
            // When my IsSelected property changes, I send a message to anyone who is listening.
            _messenger.Send(new SelectionChangedMessage<TDtoType>(this, value));
        }
    }

    # endregion
}
