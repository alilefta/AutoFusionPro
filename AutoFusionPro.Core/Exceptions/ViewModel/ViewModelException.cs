using AutoFusionPro.Core.Helpers.Operations;

namespace AutoFusionPro.Core.Exceptions.ViewModel
{
    /// <summary>
    /// Represents exceptions that occur within the View Model Layer of the application.
    /// Provides context-specific information about the view model operation failure.
    /// </summary>
    [Serializable]
    public class ViewModelException : AutoFusionProException
    {
        /// <summary>
        /// Gets the name of the view model class where the exception occurred.
        /// </summary>
        public string? ViewModelName { get; }

        /// <summary>
        /// Gets the name of the view model method where the exception occurred.
        /// </summary>
        public string? MethodName { get; }

        /// <summary>
        /// Gets the type of command or action being executed when the exception occurred.
        /// </summary>
        public string? CommandType { get; }

        /// <summary>
        /// Gets the type of operation being performed when the exception occurred (e.g., "Load", "Save", "Validate", "Navigate").
        /// </summary>
        public string? OperationType { get; }

        // Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelException"/> class with a predefined basic message.
        /// </summary>
        public ViewModelException() : base("A view model operation failed.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ViewModelException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ViewModelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelException"/> class with a specified error message and context information about the view model operation.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="viewModelName">The name of the view model class.</param>
        /// <param name="methodName">The name of the view model method.</param>
        /// <param name="commandType">The type of command or action being executed.</param>
        /// <param name="operationType">The type of operation (e.g., "Load", "Save", "Validate", "Navigate").</param>
        public ViewModelException(string message, string? viewModelName, string? methodName, MethodOperationType? operationType, string? commandType)
            : base(message)
        {
            ViewModelName = viewModelName;
            MethodName = methodName;
            OperationType = operationType.ToString();
            CommandType = commandType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelException"/> class with a specified error message, context information about the view model operation, and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="viewModelName">The name of the view model class.</param>
        /// <param name="methodName">The name of the view model method.</param>
        /// <param name="commandType">The type of command or action being executed.</param>
        /// <param name="operationType">The type of operation (e.g., "Load", "Save", "Validate", "Navigate").</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ViewModelException(string message, string? viewModelName, string? methodName, MethodOperationType? operationType, Exception innerException)
            : base(message, innerException)
        {
            ViewModelName = viewModelName;
            MethodName = methodName;
            OperationType = operationType.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelException"/> class with a specified error message, context information about the view model operation, and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="viewModelName">The name of the view model class.</param>
        /// <param name="methodName">The name of the view model method.</param>
        /// <param name="commandType">The type of command or action being executed.</param>
        /// <param name="operationType">The type of operation (e.g., "Load", "Save", "Validate", "Navigate").</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ViewModelException(string message, string? viewModelName, string? methodName, string? operationType, string? commandType, Exception innerException)
            : base(message, innerException)
        {
            ViewModelName = viewModelName;
            MethodName = methodName;
            OperationType = operationType;
            CommandType = commandType;
        }
    }
}
