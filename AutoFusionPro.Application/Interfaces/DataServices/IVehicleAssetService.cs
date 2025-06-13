using AutoFusionPro.Application.DTOs.VehicleAsset;
using AutoFusionPro.Core.Enums.ModelEnum.VehicleInventory;
using AutoFusionPro.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Application.Interfaces.DataServices
{
    /// <summary>
    /// Defines application service operations for managing comprehensive Vehicle assets.
    /// </summary>
    public interface IVehicleAssetService
    {
        #region Core Vehicle Asset CRUD

        /// <summary>
        /// Creates a new comprehensive Vehicle asset.
        /// </summary>
        /// <param name="createDto">Data for the new vehicle asset.</param>
        /// <returns>The DTO of the newly created vehicle asset with all details.</returns>
        Task<VehicleAssetDetailDto> CreateVehicleAsync(CreateVehicleAssetDto createDto);

        /// <summary>
        /// Updates the core details of an existing Vehicle asset.
        /// Does not handle collection updates (images, damage, etc.) directly; use specific methods for those.
        /// </summary>
        /// <param name="updateDto">Data containing the ID and updated core fields.</param>
        Task UpdateVehicleCoreDetailsAsync(UpdateVehicleAssetCoreDto updateDto);

        /// <summary>
        /// Gets detailed information for a specific Vehicle asset by its ID.
        /// Includes all related collections like images, damage logs, service records, and documents.
        /// </summary>
        /// <param name="vehicleId">The ID of the Vehicle asset.</param>
        /// <returns>A VehicleAssetDetailDto or null if not found.</returns>
        Task<VehicleAssetDetailDto?> GetVehicleByIdAsync(int vehicleId);

        /// <summary>
        /// Gets a Vehicle asset by its VIN, optionally including all details.
        /// </summary>
        /// <param name="vin">The Vehicle Identification Number.</param>
        /// <param name="includeAllDetails">Whether to load all related collections.</param>
        /// <returns>A VehicleAssetDetailDto (if includeAllDetails is true) or VehicleAssetSummaryDto (if false), or null.</returns>
        Task<VehicleAssetDetailDto?> GetVehicleByVinAsync(string vin, bool includeAllDetails = true); // Default to full details

        /// <summary>
        /// Gets a paginated list of Vehicle asset summaries based on filter criteria.
        /// </summary>
        Task<PagedResult<VehicleAssetSummaryDto>> GetFilteredVehiclesAsync(
            VehicleAssetFilterCriteriaDto filterCriteria, int pageNumber, int pageSize);

        /// <summary>
        /// Deletes a Vehicle asset (e.g., soft delete or change status to Scrapped).
        /// Dependent child records (images, damage logs etc.) might be cascaded or handled.
        /// </summary>
        /// <param name="vehicleId">The ID of the Vehicle asset to delete.</param>
        Task DeleteVehicleAsync(int vehicleId); // Consider if this should be ChangeVehicleStatusAsync(vehicleId, VehicleStatus.Scrapped)

        /// <summary>
        /// Changes the status of a Vehicle asset.
        /// </summary>
        Task ChangeVehicleStatusAsync(int vehicleId, VehicleStatus newStatus, string? notes);

        #endregion

        #region Vehicle Image Management

        /// <summary>
        /// Adds an image to a specific Vehicle asset.
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle.</param>
        /// <param name="imageDto">DTO containing caption, IsPrimary, DisplayOrder, and source image info.</param>
        /// <param name="imageStream">The stream of the image file to upload.</param>
        /// <param name="fileName">The original name of the image file.</param>
        /// <returns>The DTO of the added vehicle image.</returns>
        Task<VehicleImageDto> AddImageToVehicleAsync(int vehicleId, CreateVehicleImageDto imageDto, Stream imageStream, string fileName);

        /// <summary>
        /// Updates metadata for an existing vehicle image (e.g., caption, display order).
        /// </summary>
        Task UpdateVehicleImageDetailsAsync(UpdateVehicleImageDto imageDetailsDto);

        /// <summary>
        /// Removes an image from a Vehicle asset by the VehicleImage record's ID.
        /// Also deletes the physical file via IImageFileService.
        /// </summary>
        /// <param name="vehicleImageId">The ID of the VehicleImage record.</param>
        Task RemoveImageFromVehicleAsync(int vehicleImageId);

        /// <summary>
        /// Sets a specific image as the primary display image for a vehicle.
        /// Ensures other images for the same vehicle are unset as primary.
        /// </summary>
        Task SetPrimaryVehicleImageAsync(int vehicleId, int vehicleImageId);

        /// <summary>
        /// Gets all images for a specific vehicle.
        /// </summary>
        Task<IEnumerable<VehicleImageDto>> GetImagesForVehicleAsync(int vehicleId);

        #endregion

        #region Vehicle Damage Log Management

        /// <summary>
        /// Adds a new damage log entry for a vehicle.
        /// </summary>
        Task<VehicleDamageLogDto> AddDamageLogToVehicleAsync(int vehicleId, CreateVehicleDamageLogDto damageDto);

        /// <summary>
        /// Updates an existing damage log entry.
        /// </summary>
        Task UpdateDamageLogAsync(UpdateVehicleDamageLogDto damageDto);

        /// <summary>
        /// Deletes a damage log entry (and its associated damage images).
        /// </summary>
        Task DeleteDamageLogAsync(int damageLogId);

        /// <summary>
        /// Gets all damage log entries for a specific vehicle.
        /// </summary>
        Task<IEnumerable<VehicleDamageLogDto>> GetDamageLogsForVehicleAsync(int vehicleId);

        /// <summary>
        /// Adds an image specifically to a damage log entry.
        /// </summary>
        Task<VehicleDamageImageDto> AddImageToDamageLogAsync(int damageLogId, CreateVehicleImageDto imageDto, Stream imageStream, string fileName); // Reusing CreateVehicleImageDto might be okay if fields match

        /// <summary>
        /// Removes an image from a damage log entry.
        /// </summary>
        Task RemoveImageFromDamageLogAsync(int damageImageId);

        #endregion

        #region Vehicle Service Record Management

        /// <summary>
        /// Adds a new service record for a vehicle.
        /// </summary>
        Task<VehicleServiceRecordDto> AddServiceRecordToVehicleAsync(int vehicleId, CreateVehicleServiceRecordDto recordDto);

        /// <summary>
        /// Updates an existing service record.
        /// </summary>
        Task UpdateServiceRecordAsync(UpdateVehicleServiceRecordDto recordDto);

        /// <summary>
        /// Deletes a service record.
        /// </summary>
        Task DeleteServiceRecordAsync(int serviceRecordId);

        /// <summary>
        /// Gets all service records for a specific vehicle.
        /// </summary>
        Task<IEnumerable<VehicleServiceRecordDto>> GetServiceRecordsForVehicleAsync(int vehicleId);

        #endregion

        #region Vehicle Document Management

        /// <summary>
        /// Adds a new document (e.g., registration, insurance scan) to a vehicle.
        /// </summary>
        Task<VehicleDocumentDto> AddDocumentToVehicleAsync(int vehicleId, CreateVehicleDocumentDto docDto, Stream fileStream, string fileName);

        /// <summary>
        /// Updates metadata for an existing vehicle document.
        /// </summary>
        Task UpdateVehicleDocumentDetailsAsync(UpdateVehicleDocumentDto docDetailsDto);

        /// <summary>
        /// Removes a document from a Vehicle asset by the VehicleDocument record's ID.
        /// Also deletes the physical file.
        /// </summary>
        Task RemoveDocumentFromVehicleAsync(int vehicleDocumentId);

        /// <summary>
        /// Gets all documents for a specific vehicle.
        /// </summary>
        Task<IEnumerable<VehicleDocumentDto>> GetDocumentsForVehicleAsync(int vehicleId);

        #endregion

        #region Validation Helpers

        /// <summary>
        /// Checks if a VIN already exists (for a different vehicle).
        /// </summary>
        Task<bool> VinExistsAsync(string vin, int? excludeVehicleId = null);

        /// <summary>
        /// Checks if a Registration Plate Number already exists (for a different vehicle).
        /// </summary>
        Task<bool> RegistrationPlateExistsAsync(string plateNumber, string? countryOrState, int? excludeVehicleId = null);

        #endregion
    }
}
