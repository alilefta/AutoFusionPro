using AutoFusionPro.Application.DTOs.Base;
using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;

namespace AutoFusionPro.Application.Messages
{
    // A generic message for when a selectable item's state changes  
    public class SelectionChangedMessage<T> where T : class, ISelectableDto // Add constraint to ensure T implements ISelectableDto  
    {
        public SelectableItemWrapper<T> Sender { get; }
        public bool IsSelected { get; }

        public SelectionChangedMessage(SelectableItemWrapper<T> sender, bool isSelected)
        {
            Sender = sender;
            IsSelected = isSelected;
        }
    }
}
