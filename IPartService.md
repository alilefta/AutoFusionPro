# IPartService Interface Documentation

**File path:** AutoFusionPro.Application/Interfaces/DataServices/IPartService.cs

**Purpose:** This interface defines the contract for the core service responsible for managing car parts data within the AutoFusionPro backend system. It encompasses operations for CRUD (Create, Read, Update, Delete) on car parts, managing associated suppliers, validating data, handling inventory-related queries, and managing part images.

## Overview

The `IPartService` interface serves as a contract outlining the functionalities needed to manage car part entities and their related data. This includes retrieving detailed and summary information, managing relationships like suppliers and compatible vehicles, performing data validation, and handling associated images. The service plays a crucial role as the main backend service for car parts in the AutoFusionPro system.

## Key Components

### Part Core CRUD & Read Operations

These methods handle the creation, reading, updating, and soft deletion of part records. Soft deletion means the part is marked inactive rather than permanently removed, allowing for recovery.

`GetPartDetailsByIdAsync(int id)`

Retrieves detailed information about a specific part using its unique ID. This includes detailed data such as its category, related suppliers, and compatible vehicles.

`GetAllPartsSummariesAsync()`

Returns a collection of summary details for all parts. This method is intended for development testing and is planned for removal.

`GetPartDetailsByPartNumberAsync(string partNumber)`

Fetches detailed part information based on its unique part number, including its category data.

`GetFilteredPartSummariesAsync(PartFilterCriteriaDto filterCriteria, int pageNumber, int pageSize)`

Provides a paginated list of summarized part information filtered according to specific criteria. This supports browsing large datasets efficiently.

`GetLowStockPartSummariesAsync(int? topN = null)`

Gets summaries for parts that have low inventory stock levels. Optionally, it can return only the most critical *top N* parts.

`CreatePartAsync(CreatePartDto createDto)`

Creates a new car part record based on the data provided and returns the detailed information of the newly created part.

`UpdatePartAsync(UpdatePartDto updateDto)`

Updates existing core details of a car part. Note this does not affect supplier relationships or vehicle compatibilities, which have separate management methods.

`DeactivatePartAsync(int id)`

Soft deletes a part by marking it inactive rather than removing it permanently from the data store. This helps preserve historical data.

`ActivatePartAsync(int id)`

Reactivates a previously deactivated part, making it active again in the system.

### Part Supplier Management

Methods related to associating suppliers with parts, allowing for detailed supplier-specific data management such as costs and supplier part numbers.

`AddSupplierToPartAsync(int partId, PartSupplierCreateDto supplierLinkDto)`

Links a supplier to a part along with specific supplier-related details like cost and supplier part number.

`UpdateSupplierLinkForPartAsync(int supplierPartId, PartSupplierDto updateDto)`

Updates information about an existing relationship between a part and a supplier, such as price or supplier's part number.

`RemoveSupplierFromPartAsync(int supplierPartId)`

Removes the connection between a supplier and a part by deleting the specific supplier-part link identified by its ID.

`GetSuppliersForPartAsync(int partId)`

Fetches all supplier links related to a particular part, providing supplier-specific details for each.

### Validation Helpers

Functions that help ensure data integrity by checking for duplicate values related to parts.

`PartNumberExistsAsync(string partNumber, int? excludePartId = null)`

Checks whether a given part number already exists in the system, optionally excluding a specific part ID (useful during updates).

`BarcodeExistsAsync(string barcode, int? excludePartId = null)`

Verifies if a barcode already exists for any part, again optionally excluding a particular part ID, and only checks if the barcode is provided and non-empty.

### Inventory Related Operations

These methods relate to querying inventory data, potentially interacting with a separate inventory management service in the future.

`GetCurrentStockForPartAsync(int partId)`

Retrieves the current stock quantity for a specified part. This may involve querying aggregated inventory data or calling an inventory-specific service.

### Part Images Management

These methods allow managing images linked to parts, supporting operations like adding new images, updating image details, removing images, and setting a primary image.

`AddImageToPartAsync(int partId, CreatePartImageDto imageDto)`

Adds an image to a part using details provided in the input data transfer object.

`UpdatePartImageDetailsAsync(UpdatePartImageDto imageDetailsDto)`

Updates existing image properties such as captions, primary status, or display order.

`RemoveImageFromPartAsync(int partImageId)`

Deletes an image linked to a part by its unique image ID.

`SetPrimaryPartImageAsync(int partId, int partImageId)`

Marks a specific image as the primary image for the part, which might be used as the main image in display contexts.

`GetImagesForPartAsync(int partId)`

Retrieves all images currently associated with a given part.

## Usage Example

Although this is an interface without implementation, here is an example of how a developer might use an implementation of `IPartService`:

```

        // Fetch detailed info for part with ID 101
        var partDetails = await partService.GetPartDetailsByIdAsync(101);

        // Create a new car part
        var newPart = await partService.CreatePartAsync(new CreatePartDto {
          PartNumber = "AF12345",
          Name = "Brake Pad",
          CategoryId = 5,
          // other required fields...
        });

        // Update an existing part
        await partService.UpdatePartAsync(new UpdatePartDto {
          Id = newPart.Id,
          Name = "Premium Brake Pad",
          // other fields...
        });

        // Add a supplier to the part
        await partService.AddSupplierToPartAsync(newPart.Id, new PartSupplierCreateDto {
          SupplierId = 3,
          Cost = 25.99m,
          SupplierPartNumber = "BRK-PD-99"
        });

        // List parts with low stock (top 10 critical)
        var lowStockParts = await partService.GetLowStockPartSummariesAsync(10);

```

For further details, consult the AutoFusionPro documentation and the concrete implementation of this interface.