using AutoFusionPro.Application.DTOs.VehicleAsset;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Core.Enums.ModelEnum.VehicleInventory;
using AutoFusionPro.Core.Models;
using System.IO;

namespace AutoFusionPro.Application.Services.DataServices
{
    public class VehicleAssetService : IVehicleAssetService
    {
        public Task<VehicleDamageLogDto> AddDamageLogToVehicleAsync(int vehicleId, CreateVehicleDamageLogDto damageDto)
        {
            throw new NotImplementedException();
        }

        public Task<VehicleDocumentDto> AddDocumentToVehicleAsync(int vehicleId, CreateVehicleDocumentDto docDto, Stream fileStream, string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<VehicleDamageImageDto> AddImageToDamageLogAsync(int damageLogId, CreateVehicleImageDto imageDto, Stream imageStream, string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<VehicleImageDto> AddImageToVehicleAsync(int vehicleId, CreateVehicleImageDto imageDto, Stream imageStream, string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<VehicleServiceRecordDto> AddServiceRecordToVehicleAsync(int vehicleId, CreateVehicleServiceRecordDto recordDto)
        {
            throw new NotImplementedException();
        }

        public Task ChangeVehicleStatusAsync(int vehicleId, VehicleStatus newStatus, string? notes)
        {
            throw new NotImplementedException();
        }

        public Task<VehicleAssetDetailDto> CreateVehicleAsync(CreateVehicleAssetDto createDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDamageLogAsync(int damageLogId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteServiceRecordAsync(int serviceRecordId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteVehicleAsync(int vehicleId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VehicleDamageLogDto>> GetDamageLogsForVehicleAsync(int vehicleId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VehicleDocumentDto>> GetDocumentsForVehicleAsync(int vehicleId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<VehicleAssetSummaryDto>> GetFilteredVehiclesAsync(VehicleAssetFilterCriteriaDto filterCriteria, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VehicleImageDto>> GetImagesForVehicleAsync(int vehicleId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VehicleServiceRecordDto>> GetServiceRecordsForVehicleAsync(int vehicleId)
        {
            throw new NotImplementedException();
        }

        public Task<VehicleAssetDetailDto?> GetVehicleByIdAsync(int vehicleId)
        {
            throw new NotImplementedException();
        }

        public Task<VehicleAssetDetailDto?> GetVehicleByVinAsync(string vin, bool includeAllDetails = true)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegistrationPlateExistsAsync(string plateNumber, string? countryOrState, int? excludeVehicleId = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveDocumentFromVehicleAsync(int vehicleDocumentId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveImageFromDamageLogAsync(int damageImageId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveImageFromVehicleAsync(int vehicleImageId)
        {
            throw new NotImplementedException();
        }

        public Task SetPrimaryVehicleImageAsync(int vehicleId, int vehicleImageId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDamageLogAsync(UpdateVehicleDamageLogDto damageDto)
        {
            throw new NotImplementedException();
        }

        public Task UpdateServiceRecordAsync(UpdateVehicleServiceRecordDto recordDto)
        {
            throw new NotImplementedException();
        }

        public Task UpdateVehicleCoreDetailsAsync(UpdateVehicleAssetCoreDto updateDto)
        {
            throw new NotImplementedException();
        }

        public Task UpdateVehicleDocumentDetailsAsync(UpdateVehicleDocumentDto docDetailsDto)
        {
            throw new NotImplementedException();
        }

        public Task UpdateVehicleImageDetailsAsync(UpdateVehicleImageDto imageDetailsDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VinExistsAsync(string vin, int? excludeVehicleId = null)
        {
            throw new NotImplementedException();
        }
    }
}
