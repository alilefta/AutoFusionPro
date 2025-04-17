using AutoFusionPro.Core.Exceptions;
using System.Runtime.Serialization;

namespace AutoFusionPro.Core.Exceptions.Repository
{
    /// <summary>
    /// Represents exceptions that occur within the Data Access Layer (Repositories) of the application.
    /// Provides context-specific information about the repository operation failure.
    /// </summary>
    [Serializable]
    public class RepositoryException : AutoFusionProException // Inherit from a base application exception (optional, but recommended)
    {
        /// <summary>
        /// Gets the name of the repository class where the exception occurred.
        /// </summary>
        public string? RepositoryName { get; }

        /// <summary>
        /// Gets the name of the repository method where the exception occurred.
        /// </summary>
        public string? MethodName { get; }

        /// <summary>
        /// Gets the type of entity being operated on when the exception occurred.
        /// </summary>
        public string? EntityType { get; }

        /// <summary>
        /// Gets the type of repository operation being performed when the exception occurred (e.g., "Get", "Add", "Update", "Delete").
        /// </summary>
        public string? OperationType { get; }


        // Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException"/> class with a predefined basic message.
        /// </summary>
        public RepositoryException() : base("A repository operation failed.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RepositoryException(string message) : base(message)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public RepositoryException(string message, Exception innerException) : base(message, innerException)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException"/> class with a specified error message and context information about the repository operation.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="repositoryName">The name of the repository class.</param>
        /// <param name="methodName">The name of the repository method.</param>
        /// <param name="entityType">The type of entity being operated on.</param>
        /// <param name="operationType">The type of repository operation (e.g., "Get", "Add", "Update", "Delete").</param>
        public RepositoryException(string message, string? repositoryName, string? methodName, string? entityType, string? operationType)
            : base(message)
        {
            RepositoryName = repositoryName;
            MethodName = methodName;
            EntityType = entityType;
            OperationType = operationType;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException"/> class with a specified error message, context information about the repository operation, and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="repositoryName">The name of the repository class.</param>
        /// <param name="methodName">The name of the repository method.</param>
        /// <param name="entityType">The type of entity being operated on.</param>
        /// <param name="operationType">The type of repository operation (e.g., "Get", "Add", "Update", "Delete").</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public RepositoryException(string message, string? repositoryName, string? methodName, string? entityType, string? operationType, Exception innerException)
            : base(message, innerException)
        {
            RepositoryName = repositoryName;
            MethodName = methodName;
            EntityType = entityType;
            OperationType = operationType;
        }
    }
}
