using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Validators;
using AutoFusionPro.Core.Exceptions;
using AutoFusionPro.Core.Exceptions.General;
using AutoFusionPro.Core.Exceptions.Service;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Models;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Application.Services.DataServices
{
    //public class PatientService : IPatientService
    //{
    //    private IUnitOfWork _unitOfWork { get; }
    //    private PatientValidator _patientValidator { get; }
    //    private ILogger<PatientService> _logger { get; }


    //    public PatientService(IUnitOfWork unitOfWork, PatientValidator patientValidator, ILogger<PatientService> logger)
    //    {
    //        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(IUnitOfWork));
    //        _patientValidator = patientValidator ?? throw new ArgumentNullException(nameof(PatientValidator));
    //        _logger = logger ?? throw new ArgumentNullException(nameof(ILogger));
    //    }

    //    public async Task<Patient> AddPatientAsync(Patient patient)
    //    {
    //        try
    //        {
    //            // Validate patient data
    //            var validationResult = await _patientValidator.ValidateAsync(patient);
    //            if (!validationResult.IsValid)
    //            {
    //                throw new ValidationException(validationResult.Errors);
    //            }

    //            // Set created date
    //            patient.CreatedAt = DateTime.Now;

    //            // Add to repository
    //            await _unitOfWork.Patients.AddAsync(patient);

    //            // Save changes via unit of work
    //            await _unitOfWork.SaveChangesAsync();

    //            _logger.LogInformation("Added new patient: {PatientId} - {PatientName}",
    //                patient.Id, $"{patient.Name}");

    //            return patient;
    //        }
    //        catch (ValidationException)
    //        {
    //            // Re throw validation exceptions as-is
    //            throw;
    //        }
    //        catch (Exception ex) when (ex is not NotFoundException)
    //        {
    //            _logger.LogError(ex, "Error adding new patient: {PatientName}",
    //                $"{patient.Name}");
    //            throw new ServiceException(
    //                $"Failed to add new patient: {patient.Name}",
    //                nameof(PatientService),
    //                nameof(AddPatientAsync),
    //                "Add",
    //                ex);
    //        }
    //    }


    //    //public async Task CreatePatientWithDocumentsAsync(Patient patient, PatientDocument document)
    //    //{
    //    //    try
    //    //    {
    //    //        // Begin transaction
    //    //        await _unitOfWork.BeginTransactionAsync();

    //    //        // Add patient through repository
    //    //        await _unitOfWork.Patients.AddAsync(patient);

    //    //        // Save to get the patient ID
    //    //        await _unitOfWork.SaveChangesAsync();

    //    //        // Set the patient ID on the document
    //    //        document.PatientId = patient.Id;

    //    //        // Add document through repository
    //    //        await _unitOfWork.PatientDocuments.AddAsync(document);

    //    //        // Save all changes at once
    //    //        await _unitOfWork.SaveChangesAsync();

    //    //        // Commit the transaction
    //    //        await _unitOfWork.CommitTransactionAsync();
    //    //    }
    //    //    catch (Exception)
    //    //    {
    //    //        // Rollback on any error
    //    //        await _unitOfWork.RollbackTransactionAsync();
    //    //        throw;
    //    //    }
    //    //}

    //    public async Task<Patient> GetPatientByIdAsync(int id)
    //    {
    //        try
    //        {
    //            var patient = await _unitOfWork.Patients.GetByIdAsync(id);
    //            if (patient == null)
    //            {
    //                _logger.LogWarning("Patient with ID {PatientId} was not found", id);
    //                throw new NotFoundException($"Patient with ID {id} not found");
    //            }
    //            return patient;

    //        }catch (NotFoundException ex) {

    //            _logger.LogError(ex, "patient with ID {PatientId} was not found.", id);
    //            throw new DentaFusionProException("patient was not found.", ex);

    //        }
    //        catch (Exception ex) when (ex is not NotFoundException)
    //        {
    //            _logger.LogError(ex, "Error retrieving patient with ID {PatientId}", id);
    //            throw new DentaFusionProException("Failed to retrieve patient", ex);
    //        }

    //    }

    //    public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
    //    {
    //        try
    //        {
    //            return await _unitOfWork.Patients.GetAllAsync();
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error retrieving all patients");
    //            throw new DentaFusionProException("Failed to retrieve patients", ex);
    //        }
    //    }

    //    public async Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm)
    //    {
    //        try
    //        {
    //            if (string.IsNullOrWhiteSpace(searchTerm))
    //            {
    //                return await GetAllPatientsAsync();
    //            }

    //            return await _unitOfWork.Patients.SearchPatientsAsync(searchTerm);
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error searching patients with term {SearchTerm}", searchTerm);
    //            throw new ServiceException($"Failed to search patients", 
    //                nameof(PatientService), 
    //                nameof(SearchPatientsAsync),  
    //                "Get",
    //                ex);
    //        }
    //    }

    //    public Task<Patient> UpdatePatientAsync(Patient patient)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public async Task<bool> DeletePatientAsync(int id)
    //    {
    //        try
    //        {
    //            // Check if patient exists
    //            var existingPatient = await _unitOfWork.Patients.GetByIdAsync(id);
    //            if (existingPatient == null)
    //            {
    //                _logger.LogWarning("Cannot delete patient with ID {PatientId} - not found", id);
    //                throw new NotFoundException($"Patient with ID {id} not found");
    //            }

    //            // Check if patient has related data that would prevent deletion
    //            // You might want to implement custom checks here

    //            // Delete from repository
    //            await _unitOfWork.Patients.DeleteAsync(id);

    //            // Save changes via unit of work
    //            await _unitOfWork.SaveChangesAsync();

    //            _logger.LogInformation("Deleted patient: {PatientId}", id);

    //            return true;
    //        }
    //        catch (NotFoundException)
    //        {
    //            // Re throw not found exceptions as-is
    //            throw new NotFoundException($"Patient with ID {id} was not found");
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error deleting patient: {PatientId}", id);
    //            throw new ServiceException($"Failed to delete patient", ex);
    //        }
    //    }

    //    public async Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(int patientId)
    //    {
    //        try
    //        {
    //            // Check if patient exists
    //            var existingPatient = await _unitOfWork.Patients.GetByIdAsync(patientId);
    //            if (existingPatient == null)
    //            {
    //                _logger.LogWarning("Cannot get appointments for patient with ID {PatientId} - patient not found", patientId);
    //                throw new NotFoundException($"Patient with ID {patientId} not found");
    //            }

    //            return await _unitOfWork.Patients.GetPatientAppointmentsAsync(patientId);
    //        }
    //        catch (NotFoundException)
    //        {
    //            throw new NotFoundException($"Patient with ID {patientId} was not found.");
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error retrieving appointments for patient: {PatientId}", patientId);
    //            throw new ServiceException($"Failed to retrieve patient appointments", ex);
    //        }
    //    }

    //    public async Task<IEnumerable<TreatmentPlan>> GetPatientTreatmentPlansAsync(int patientId)
    //    {
    //        try
    //        {
    //            // Check if patient exists
    //            var existingPatient = await _unitOfWork.Patients.GetByIdAsync(patientId);
    //            if (existingPatient == null)
    //            {
    //                _logger.LogWarning("Cannot get treatment plans for patient with ID {PatientId} - patient not found", patientId);
    //                throw new NotFoundException($"Patient with ID {patientId} not found");
    //            }

    //            return await _unitOfWork.Patients.GetPatientTreatmentPlansAsync(patientId);
    //        }
    //        catch (NotFoundException)
    //        {
    //            throw new NotFoundException($"Patient with ID {patientId} was not found.");
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error retrieving treatment plans for patient: {PatientId}", patientId);
    //            throw new ServiceException($"Failed to retrieve patient treatment plans", ex);
    //        }
    //    }
    //}

}
