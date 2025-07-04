# IVehicleAssetService Interface Documentation

This interface defines the core set of operations for managing comprehensive vehicle assets within the AutoFusionPro application. It provides a contract for creating, updating, retrieving, and deleting vehicle asset data, as well as managing related entities like images, damage logs, service records, and documents. The interface supports asynchronous operations for efficient data processing and integration.

## Overview

The `IVehicleAssetService` interface encapsulates all functionalities related to vehicle asset management in a modular and organized manner. Its methods allow applications to handle core vehicle data as well as auxiliary information such as images and maintenance records. This interface promotes separation of concerns and facilitates maintainability and scalability for vehicle inventory management systems.

## Core Vehicle Asset CRUD Operations

This group of methods focuses on Create, Read, Update, and Delete operations related to vehicle assets themselves, excluding collections like images or documents.

**CreateVehicleAsync**

Asynchronously creates a new vehicle asset with comprehensive details.

**Input:** A data transfer object (DTO) that carries information needed to create the vehicle.

**Output:** A detailed DTO of the newly created vehicle asset.

*Example usage:* Adding a new car to the inventory by sending the required vehicle properties to this method.

**UpdateVehicleCoreDetailsAsync**

Updates the primary attributes of an existing vehicle asset, such as model, year, or status.

This does not affect supplementary collections like images or damage logs; for those, dedicated methods exist.

**GetVehicleByIdAsync**

Retrieves detailed information about a vehicle asset by its unique identifier.

The returned data includes all related data, such as images, damage logs, service records, and documents.

Returns null if no vehicle matches the provided ID.

**GetVehicleByVinAsync**

Fetches a vehicle asset by its Vehicle Identification Number (VIN).

Optionally, it includes all detailed data collections or just a summary.

By default, detailed information is retrieved.

**GetFilteredVehiclesAsync**

Fetches a paginated list of summarized vehicle assets filtered based on provided criteria.

Supports specifying page number and size for efficient data browsing.

**DeleteVehicleAsync**

Deletes a vehicle asset, which could represent a soft delete or status change (e.g., to Scrapped).

It may cascade deletion or mark related child records appropriately.

**ChangeVehicleStatusAsync**

Changes the status of a vehicle asset such as Active, Sold, or Scrapped, and optionally records notes about the status change.

## Vehicle Image Management

These methods allow managing images associated with vehicle assets, including their upload, metadata updates, removal, and setting primary display images.

**AddImageToVehicleAsync**

Adds a new image to a specific vehicle asset.

Expects the vehicle’s ID, image metadata (caption, order, etc.), and the image stream and filename.

Returns a DTO representing the newly added image.

**UpdateVehicleImageDetailsAsync**

Updates the metadata of an existing vehicle image, such as the caption or display order.

**RemoveImageFromVehicleAsync**

Removes an image from a vehicle by its image record ID.

It also handles deletion of the physical image file through an external image file service.

**SetPrimaryVehicleImageAsync**

Designates a specific image as the primary display image for the vehicle.

Ensures all other images for that vehicle are marked as non-primary.

**GetImagesForVehicleAsync**

Retrieves all images associated with a particular vehicle.

## Vehicle Damage Log Management

These methods manage logs and images related to damage entries for vehicles.

**AddDamageLogToVehicleAsync**

Adds a new damage log entry describing any damages found on the vehicle.

**UpdateDamageLogAsync**

Updates details of an existing damage log entry.

**DeleteDamageLogAsync**

Deletes a damage log entry and any associated damage images.

**GetDamageLogsForVehicleAsync**

Fetches all damage log entries pertaining to a specific vehicle.

**AddImageToDamageLogAsync**

Adds an image related specifically to a damage log entry.

Reuses vehicle image DTOs for convenience.

**RemoveImageFromDamageLogAsync**

Removes a damage image using its unique identifier.

## Vehicle Service Record Management

This collection of methods manages the service and maintenance history linked to vehicles.

**AddServiceRecordToVehicleAsync**

Creates a new service record for a vehicle, such as oil changes or inspections.

**UpdateServiceRecordAsync**

Updates an existing service record's details.

**DeleteServiceRecordAsync**

Removes a service record from a vehicle's history.

**GetServiceRecordsForVehicleAsync**

Retrieves all service records associated with a vehicle.

## Vehicle Document Management

These methods oversee documents tied to vehicles, like registrations and insurance papers.

**AddDocumentToVehicleAsync**

Adds a new document to a vehicle, with file contents and metadata.

Typically used for scans of registration, insurance, or ownership documents.

**UpdateVehicleDocumentDetailsAsync**

Updates metadata for an existing vehicle document.

**RemoveDocumentFromVehicleAsync**

Removes a vehicle document by its record ID and deletes the physical file.

**GetDocumentsForVehicleAsync**

Retrieves all documents related to a specified vehicle.

## Validation Helpers

These utility methods help ensure data integrity by avoiding duplicates.

**VinExistsAsync**

Checks asynchronously if a given VIN already exists for a different vehicle in the system.

Useful to prevent duplicate vehicle entries.

**RegistrationPlateExistsAsync**

Checks if a registration plate number exists for another vehicle, optionally filtered by country or state.

This also helps avoid duplicate records and maintain data consistency.

This interface embodies the backbone logic for vehicle asset management functionality in AutoFusionPro, serving as a contract for repository or service implementations to build upon. It enables robust handling of vehicle information and related data, supporting better vehicle lifecycle and inventory management.