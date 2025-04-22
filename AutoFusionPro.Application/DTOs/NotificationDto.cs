using AutoFusionPro.Core.Enums.UI;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoFusionPro.Application.DTOs
{
    // Inherit from ObservableObject to get INotifyPropertyChanged implementation
    public partial class NotificationDto : ObservableObject
    {
        // Private backing fields
        private int _id;
        private string _title;
        private string _message;
        private DateTime _timestamp;
        private bool _isRead; // This property will now be mutable
        private NotificationType _type;
        private string? _relatedEntityId;

        // Public properties using [ObservableProperty] for auto-generation
        // or manual implementation with SetProperty()

        [ObservableProperty]
        private int id; // Generates public int Id { get => _id; set => SetProperty(ref _id, value); }

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string message;

        [ObservableProperty]
        private DateTime timestamp; // Assuming timestamp itself doesn't change after creation

        // Make IsRead mutable and notify changes
        [ObservableProperty]
        private bool isRead;

        [ObservableProperty]
        private NotificationType type; // Assuming type doesn't change

        [ObservableProperty]
        private string? relatedEntityId; // Assuming related ID doesn't change

        // --- Constructor ---
        // You need a constructor to initialize the object.
        // Unlike positional records, classes need explicit constructors.
        public NotificationDto(
            int id,
            string title,
            string message,
            DateTime timestamp,
            bool isRead,
            NotificationType type,
            string? relatedEntityId)
        {
            // Initialize backing fields or properties directly
            // Using properties is fine here as SetProperty handles initial set correctly
            Id = id;
            Title = title;
            Message = message;
            Timestamp = timestamp;
            IsRead = isRead; // Set initial read status
            Type = type;
            RelatedEntityId = relatedEntityId;
        }

        // --- Optional: Add a parameterless constructor ONLY if needed by some libraries (like certain serializers) ---
        // public NotificationDto() { } // Be careful with this, ensure properties are initialized elsewhere if used.
    }
}
