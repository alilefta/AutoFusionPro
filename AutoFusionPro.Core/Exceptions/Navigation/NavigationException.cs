using AutoFusionPro.Core.Enums.NavigationPages;

namespace AutoFusionPro.Core.Exceptions.Navigation
{
    public class NavigationException : AutoFusionProException
    {
        // The page that caused the navigation error
        public ApplicationPage Page { get; }
        public object InitializationParameter { get; }

        // Constructor with just a message
        public NavigationException(string message)
            : base(message)
        {
        }

        // Constructor with message and inner exception
        public NavigationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // Constructor with page, message, and inner exception
        public NavigationException(ApplicationPage page, string message, Exception innerException)
            : base(message, innerException)
        {
            Page = page;
        }

        public NavigationException(
            ApplicationPage page,
            string message,
            object initializationParameter = null,
            Exception innerException = null)
        : base(message, innerException)
        {
            Page = page;
            InitializationParameter = initializationParameter;
        }
    }
}
