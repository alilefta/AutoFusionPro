# IVehicleTaxonomyService Interface Documentation

**Location:** AutoFusionPro.Application/Interfaces/DataServices/IVehicleTaxonomyService.cs

## Overview

This interface defines a collection of asynchronous service operations for managing vehicle taxonomy data within the AutoFusionPro application. The vehicle taxonomy includes makes, models, trim levels, transmission types, engine types, and body types. These operations enable retrieval, creation, updating, and deletion of vehicle-related entities, allowing the application to manage and configure CompatibleVehicle setups. Such configurations are crucial for determining the compatibility of automotive parts with specific vehicles.

Although the interface originally anticipated operations for managing CompatibleVehicle entities themselves (such as creating, updating, deleting, and searching with pagination), these methods are currently commented out, indicating they might be under revision or temporarily disabled.

## Interface Methods

### Make Management

Operations related to the management of car manufacturers (Makes).

- `GetAllMakesAsync`
    
    Retrieves a list of all vehicle makes.
    
- `GetMakeByIdAsync`
    
    Fetches the details of a specific make by its identifier. Returns `null` if not found.
    
- `CreateMakeAsync`
    
    Creates a new make with the data provided in the `CreateMakeDto`.
    
- `UpdateMakeAsync`
    
    Updates an existing make based on the provided `UpdateMakeDto`.
    
- `DeleteMakeAsync`
    
    Deletes a make by its identifier. It is important to check for dependent models before deletion to avoid data inconsistencies.
    

### Model Management

Operations related to vehicle models, typically associated with a make.

- `GetModelsByMakeIdAsync`
    
    Retrieves all models belonging to a specific make.
    
- `GetModelByIdAsync`
    
    Gets a particular model's details by its ID, may return `null` if not found.
    
- `CreateModelAsync`
    
    Creates a new model using the provided data transfer object.
    
- `UpdateModelAsync`
    
    Updates model information.
    
- `DeleteModelAsync`
    
    Deletes a model. Before deletion, ensure no dependent trim levels or compatible vehicle configurations exist.
    

### Trim Level Management

Trim levels define specific configurations or packages for vehicle models.

- `GetTrimLevelsByModelIdAsync`
    
    Fetches all trim levels under a given model.
    
- `GetTrimLevelByIdAsync`
    
    Gets details of a specific trim level, may return `null`.
    
- `CreateTrimLevelAsync`
    
    Creates a trim level entry.
    
- `UpdateTrimLevelAsync`
    
    Updates trim level details.
    
- `DeleteTrimLevelAsync`
    
    Deletes a trim level after ensuring there are no dependent compatible vehicles.
    

### Transmission Type Management

Manages types of vehicle transmission such as manual or automatic.

- `GetAllTransmissionTypesAsync`
    
    Lists all transmission types.
    
- `GetTransmissionTypeByIdAsync`
    
    Gets a transmission type by its ID.
    
- `CreateTransmissionTypeAsync`
    
    Creates a transmission type using a generic lookup DTO, useful for simple data entries.
    
- `UpdateTransmissionTypeAsync`
    
    Updates an existing transmission type.
    
- `DeleteTransmissionTypeAsync`
    
    Deletes a transmission type, ensuring no dependent compatible vehicles exist.
    

### Engine Type Management

Operations for managing engine types, such as V6, V8, etc.

- `GetAllEngineTypesAsync`
    
    Retrieves all engine types.
    
- `GetEngineTypeByIdAsync`
    
    Gets an engine type by ID.
    
- `CreateEngineTypeAsync`
    
    Creates a new engine type with specific data, including possibly a unique code.
    
- `UpdateEngineTypeAsync`
    
    Updates engine type details.
    
- `DeleteEngineTypeAsync`
    
    Deletes an engine type, checking for dependent compatible vehicles before removal.
    

### Body Type Management

Manages various vehicle body types such as sedan, SUV, hatchback, etc.

- `GetAllBodyTypesAsync`
    
    Fetches all body types.
    
- `GetBodyTypeByIdAsync`
    
    Retrieves a specific body type by its ID.
    
- `CreateBodyTypeAsync`
    
    Creates a new body type with a generic lookup DTO.
    
- `UpdateBodyTypeAsync`
    
    Updates body type information.
    
- `DeleteBodyTypeAsync`
    
    Deletes a body type, ensuring that no compatible vehicles depend on it.
    

## Additional Notes

- All methods in this interface are asynchronous, returning a `Task` or `Task<T>`. This design supports non-blocking operations common in modern applications dealing with I/O or database interactions.
- The Create, Update, and Delete methods typically take Data Transfer Objects (`Dto`) which encapsulate the data required for the operation. This encourages separation of concerns and helps maintain a clean architecture.
- Comments within the interface indicate that dependency checks should be performed before deletion of entities to protect data integrity. This might involve ensuring related child records (such as models related to a make or compatible vehicles linked to a trim level) are handled appropriately.

## Example Usage Scenario

Imagine you are developing a feature to add a new vehicle make to AutoFusionPro. Using this interface, your service layer might involve the following steps:

1. Create a `CreateMakeDto` object containing the make's details (e.g., name, country).
2. Invoke `CreateMakeAsync` with the DTO to add the new make.
3. After creation, you might call `GetAllMakesAsync` to refresh your UI with the updated list of makes.
4. When deleting a make, first ensure that there are no models linked to that make to avoid orphaned data.