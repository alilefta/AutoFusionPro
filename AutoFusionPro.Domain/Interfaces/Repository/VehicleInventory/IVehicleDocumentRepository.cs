using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models.VehiclesInventory;

namespace AutoFusionPro.Domain.Interfaces.Repository.VehicleInventory
{
    /// <summary>
    /// Repository interface for managing VehicleDocument entities.
    /// </summary>
    public interface IVehicleDocumentRepository : IBaseRepository<VehicleDocument>
    {
        /// <summary>
        /// Gets all documents associated with a specific vehicle.
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle.</param>
        /// <returns>A collection of VehicleDocument entities for the specified vehicle.</returns>
        Task<IEnumerable<VehicleDocument>> GetByVehicleIdAsync(int vehicleId);

        // Potentially other specific methods if needed, e.g.:
        // Task<VehicleDocument?> GetByVehicleIdAndDocumentTypeAsync(int vehicleId, DocumentType documentType);
        // Task<bool> DocumentExistsForVehicleAsync(int vehicleId, string documentName);
    }
}
