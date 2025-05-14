using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Core.Exceptions.Validation
{
    /// <summary>
    /// Represents errors that occur when an attempt to delete an entity is blocked
    /// due to existing associations with other entities (referential integrity).
    /// </summary>
    [Serializable]
    public class DeletionBlockedException : AutoFusionProException
    {
        /// <summary>
        /// Gets the name of the entity type that was attempted to be deleted.
        /// </summary>
        public string? EntityName { get; }

        /// <summary>
        /// Gets the ID or key of the entity that was attempted to be deleted.
        /// </summary>
        public string? EntityKey { get; }

        /// <summary>
        /// Gets a collection of entity types that are dependent on the entity being deleted,
        /// preventing its deletion.
        /// </summary>
        public IEnumerable<string>? DependentEntityTypes { get; }

        // Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletionBlockedException"/> class with a predefined basic message.
        /// </summary>
        public DeletionBlockedException()
            : base("The requested item cannot be deleted because it is associated with other records.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletionBlockedException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DeletionBlockedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletionBlockedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public DeletionBlockedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletionBlockedException"/> class with a specified error message and context about the deletion attempt.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="entityName">The name of the entity type attempted to be deleted.</param>
        /// <param name="entityKey">The ID or key of the entity attempted to be deleted.</param>
        /// <param name="dependentEntityTypes">A collection of names of entity types that depend on this entity.</param>
        public DeletionBlockedException(string message, string? entityName, string? entityKey, IEnumerable<string>? dependentEntityTypes = null)
            : base(message)
        {
            EntityName = entityName;
            EntityKey = entityKey;
            DependentEntityTypes = dependentEntityTypes ?? Enumerable.Empty<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletionBlockedException"/> class with context and a reference to the inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="entityName">The name of the entity type attempted to be deleted.</param>
        /// <param name="entityKey">The ID or key of the entity attempted to be deleted.</param>
        /// <param name="dependentEntityTypes">A collection of names of entity types that depend on this entity.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public DeletionBlockedException(string message, string? entityName, string? entityKey, IEnumerable<string>? dependentEntityTypes, Exception innerException)
            : base(message, innerException)
        {
            EntityName = entityName;
            EntityKey = entityKey;
            DependentEntityTypes = dependentEntityTypes ?? Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets a message that describes the current exception, including specific deletion blocking details if available.
        /// </summary>
        public override string Message
        {
            get
            {
                var baseMessage = base.Message;
                var details = new List<string>();

                if (!string.IsNullOrEmpty(EntityName))
                {
                    details.Add($"Entity: '{EntityName}' (Key: {EntityKey ?? "N/A"})");
                }

                if (DependentEntityTypes != null && DependentEntityTypes.Any())
                {
                    details.Add($"Dependent Entities: {string.Join(", ", DependentEntityTypes.Select(s => $"'{s}'"))}");
                }

                if (details.Any())
                {
                    return $"{baseMessage} Details: {string.Join("; ", details)}.";
                }
                return baseMessage;
            }
        }
    }
}
