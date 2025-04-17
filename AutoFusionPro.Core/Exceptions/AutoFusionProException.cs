using System.Runtime.Serialization;

namespace AutoFusionPro.Core.Exceptions
{
    /// <summary>
    /// Serves as the base class for all custom exceptions defined within the AutoFusionPro application.
    /// Provides a common root for application-specific exceptions, allowing for consistent exception handling.
    /// </summary>
    [Serializable]
    public class AutoFusionProException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoFusionProException"/> class.
        /// </summary>
        public AutoFusionProException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoFusionProException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AutoFusionProException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoFusionProException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public AutoFusionProException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Serialization constructor for <see cref="AutoFusionProException"/>.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    }
}
