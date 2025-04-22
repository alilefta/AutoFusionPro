using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Core.Enums.UI;

namespace AutoFusionPro.Application.Events
{
    public class NotificationChangedEventArgs : EventArgs
    {
        public UserRole AffectedRole { get; set; }
        public NotificationChangeType ChangeType { get; set; }
    }
}
