namespace AutoFusionPro.Core.Exceptions.Validation
{
    /// <summary>
    /// Represents errors that occur when an operation attempts to create or update data
    /// that would violate a uniqueness constraint (i.e., create a duplicate).
    /// </summary>
    [Serializable]
    public class DuplicationException : AutoFusionProException
    {
        /// <summary>
        /// Gets the name of the entity type that caused the duplication.
        /// </summary>
        public string? EntityName { get; }

        /// <summary>
        /// Gets the name of the property or field(s) that caused the duplication.
        /// Can be a single property name or a comma-separated list if a composite key is involved.
        /// </summary>
        public string? ConflictingProperty { get; }

        /// <summary>
        /// Gets the value(s) that caused the duplication.
        /// Can be a single value or a comma-separated list corresponding to ConflictingProperty.
        /// </summary>
        public string? ConflictingValue { get; }

        // Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicationException"/> class with a predefined basic message.
        /// </summary>
        public DuplicationException()
            : base("An attempt was made to create a duplicate record.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DuplicationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public DuplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicationException"/> class with a specified error message and context about the duplication.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="entityName">The name of the entity type involved in the duplication.</param>
        /// <param name="conflictingProperty">The name of the property or properties that caused the duplication.</param>
        /// <param name="conflictingValue">The value or values that caused the duplication.</param>
        public DuplicationException(string message, string? entityName, string? conflictingProperty, string? conflictingValue)
            : base(message)
        {
            EntityName = entityName;
            ConflictingProperty = conflictingProperty;
            ConflictingValue = conflictingValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicationException"/> class with a specified error message, context about the duplication, and a reference to the inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="entityName">The name of the entity type involved in the duplication.</param>
        /// <param name="conflictingProperty">The name of the property or properties that caused the duplication.</param>
        /// <param name="conflictingValue">The value or values that caused the duplication.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public DuplicationException(string message, string? entityName, string? conflictingProperty, string? conflictingValue, Exception innerException)
            : base(message, innerException)
        {
            EntityName = entityName;
            ConflictingProperty = conflictingProperty;
            ConflictingValue = conflictingValue;
        }

        /// <summary>
        /// Gets a message that describes the current exception, including specific duplication details if available.
        /// </summary>
        public override string Message
        {
            get
            {
                if (!string.IsNullOrEmpty(EntityName) && !string.IsNullOrEmpty(ConflictingProperty))
                {
                    return $"{base.Message} Entity: '{EntityName}', Property: '{ConflictingProperty}', Value: '{ConflictingValue}'.";
                }
                return base.Message;
            }
        }
    }
}
