using AutoFusionPro.Core.Exceptions;
using System.Runtime.Serialization;

namespace AutoFusionPro.Core.Exceptions.Service
{
    /// <summary>
    /// Represents exceptions that occur within the Service Layer of the application.
    /// Provides context-specific information about the service operation failure.
    /// </summary>
    [Serializable]
    public class ServiceException : AutoFusionProException
    {
        /// <summary>
        /// Gets the name of the service class where the exception occurred.
        /// </summary>
        public string? ServiceName { get; }

        /// <summary>
        /// Gets the name of the service method where the exception occurred.
        /// </summary>
        public string? MethodName { get; }

        /// <summary>
        /// Gets the type of operation being performed when the exception occurred.
        /// </summary>
        public string? OperationType { get; }

        // Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class with a predefined basic message.
        /// </summary>
        public ServiceException() : base("A service operation failed.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ServiceException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class with a specified error message and context information about the service operation.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="serviceName">The name of the service class.</param>
        /// <param name="methodName">The name of the service method.</param>
        /// <param name="operationType">The type of service operation.</param>
        public ServiceException(string message, string? serviceName, string? methodName, string? operationType)
            : base(message)
        {
            ServiceName = serviceName;
            MethodName = methodName;
            OperationType = operationType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class with a specified error message, context information about the service operation, and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="serviceName">The name of the service class.</param>
        /// <param name="methodName">The name of the service method.</param>
        /// <param name="operationType">The type of service operation.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ServiceException(string message, string? serviceName, string? methodName, string? operationType, Exception innerException)
            : base(message, innerException)
        {
            ServiceName = serviceName;
            MethodName = methodName;
            OperationType = operationType;
        }

    }
}
