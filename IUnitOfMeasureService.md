# Documentation for IUnitOfMeasureService Interface

**File Location:** AutoFusionPro.Application/Interfaces/DataServices/IUnitOfMeasureService.cs

## Overview

This interface defines a contract for managing Units of Measure within the AutoFusionPro application. It provides asynchronous operations for creating, retrieving, updating, and deleting unit of measure records. These operations are essential for handling measurement units consistently across the software, which may be used in inventory, parts categorization, or other modules requiring standardized units.

The interface uses Data Transfer Objects (DTOs) to encapsulate the data for units of measure and relies on asynchronous tasks to improve performance and responsiveness in applications such as web apps or services.

## Interface: IUnitOfMeasureService

This interface outlines six primary asynchronous methods for unit of measure management:

### GetUnitOfMeasureByIdAsync

**Purpose:** Retrieve a specific Unit of Measure by its unique numeric identifier.

**Parameters:** `id` - an `int` representing the Unit of Measure ID.

**Returns:** A `UnitOfMeasureDto` object representing the unit if found, or `null` otherwise.

**Use case example:** When you need to display details about a particular unit for viewing or editing.

### GetAllUnitOfMeasuresAsync

**Purpose:** Fetch all available Units of Measure, typically used to populate dropdowns or selection controls.

**Returns:** An enumerable collection of `UnitOfMeasureDto` objects.

**Additional details:** The units are usually ordered by their name to make selection easier for the end user.

**Use case example:** Filling a combo box with measurement units when creating or editing parts.

### CreateUnitOfMeasureAsync

**Purpose:** Create a new Unit of Measure record in the system.

**Parameters:** `createDto` - a `CreateUnitOfMeasureDto` object containing details such as name and symbol for the new unit.

**Returns:** The newly created `UnitOfMeasureDto`, including its assigned identifier.

**Exceptions:**

- `FluentValidation.ValidationException` if validation rules fail, e.g., non-unique name or symbol.
- `ServiceException` for other failures during creation.

**Use case example:** Adding a new measurement unit when the current set is insufficient or a new category arises.

### UpdateUnitOfMeasureAsync

**Purpose:** Update details of an existing Unit of Measure.

**Parameters:** `updateDto` - an `UpdateUnitOfMeasureDto` that includes the ID of the unit to update along with new values.

**Returns:** A `Task` representing the asynchronous operation with no result.

**Exceptions:**

- `FluentValidation.ValidationException` if input data is invalid.
- `ServiceException` if the unit is not found or another error occurs.

**Use case example:** Correcting the name or symbol of a unit after a typo or business rule change.

### DeleteUnitOfMeasureAsync

**Purpose:** Remove a Unit of Measure from the system, ensuring it is not currently used by any dependent objects like parts.

**Parameters:** `id` - the identifier of the Unit of Measure to delete.

**Returns:** A `Task` indicating completion of the deletion process.

**Exceptions:** Throws `ServiceException` if the unit does not exist or cannot be deleted due to existing dependencies.

**Use case example:** Cleaning up unused or deprecated units to keep the system tidy and consistent.

## Additional Notes

The interface includes some commented-out optional helpers for checking uniqueness of unit names or symbols. Although these are not currently exposed, they indicate a consideration for validation steps that might be performed either internally or directly by the user interface or other services.

By keeping these services asynchronous and well-defined, the system ensures smooth integration with UI layers or other backend services without blocking operations.

This documentation aims to provide a clear understanding of the responsibilities and usage of the `IUnitOfMeasureService` interface for new developers contributing to the AutoFusionPro project. Proper use of this interface helps maintain consistent measurement units across the application.