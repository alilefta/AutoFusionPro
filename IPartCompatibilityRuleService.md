# IPartCompatibilityRuleService Interface Documentation

Welcome! This documentation provides a clear and friendly explanation of the `IPartCompatibilityRuleService` interface in the AutoFusionPro application. This interface defines various methods to manage and query part compatibility rules — a key aspect for determining what vehicle parts fit with specific vehicle configurations.

## Overview

The `IPartCompatibilityRuleService` interface lays out the contract for services that handle compatibility rules linked to automotive parts. These rules enable the system to identify which parts are compatible with particular vehicle specifications, supporting functionalities such as:

- Creating, updating, and deleting compatibility rules for parts.
- Retrieving compatibility rule details for specific parts or rules.
- Finding parts compatible with detailed vehicle specifications.
- Checking compatibility on an individual part-vs.-vehicle level.

The interface supports asynchronous operations to ensure smooth performance in applications that may process many compatibility checks or modifications simultaneously.

## Methods

### CreateRuleForPartAsync

This method adds a new compatibility rule associated with a specific part.

**Parameters:**

- `partId` (int): The identifier of the part to attach the rule to.
- `createRuleDto` (CreatePartCompatibilityRuleDto): An object containing the data needed to define the new rule.

**Returns:** A detailed data transfer object (DTO) representing the newly created rule.

*Example:* To specify that a brake rotor fits a specific car make and model, you would call this method with the brake rotor’s ID and the rule details encapsulated in `createRuleDto`.

### UpdateRuleAsync

Updates an existing compatibility rule based on data provided.

**Parameters:**

- `updateRuleDto` (UpdatePartCompatibilityRuleDto): Contains the ID of the rule to update and the new details.

**Returns:** This is a void asynchronous operation (Task) that completes when the update is done.

### DeleteRuleAsync

Deletes a compatibility rule identified by its unique ID.

**Parameter:**

- `ruleId` (int): The ID of the rule you want to remove.

**Returns:** An asynchronous task indicating completion of the deletion.

### GetRuleByIdAsync

Retrieves complete details of a specific compatibility rule by its ID.

**Parameter:**

- `ruleId` (int): The rule’s unique identifier.

**Returns:** A detailed DTO of the rule if found, or `null` otherwise.

### GetRulesForPartAsync

Fetches all compatibility rules linked to a specific part, optionally filtering by only active rules.

**Parameters:**

- `partId` (int): The ID of the part whose rules you want to retrieve.
- `onlyActiveRules` (bool): Optional flag (defaults to true) to restrict results to active rules only.

**Returns:** A collection of summarized DTOs representing the part’s compatibility rules.

### FindPartIdsMatchingVehicleSpecAsync

Finds distinct part IDs that have active compatibility rules fitting a specific vehicle specification.

**Parameters:**

- `vehicleSpec` (VehicleSpecificationDto): A detailed description of the vehicle (like make, model, year, trim).
- `ruleAndPartFilters` (PartCompatibilityRuleFilterDto, optional): Additional criteria to narrow down the parts or rules (e.g. only active parts, certain categories).

**Returns:** A collection of distinct part IDs that match the vehicle specification and filters.

*Example:* Suppose you have a vehicle with certain attributes and want to find all parts that fit it — you would use this method to get a list of candidate part IDs quickly.

### IsPartCompatibleAsync

Checks whether a single part is compatible with a specific vehicle based on its detailed specification.

**Parameters:**

- `partId` (int): The part’s unique ID.
- `vehicleSpec` (VehicleSpecificationDto): The detailed vehicle data used to verify compatibility.

**Returns:** A DTO describing whether the part is compatible and explains the matching result.

*Example:* Use this to confirm if a particular tire fits a given vehicle configuration.

## Future and Deprecated Features

The interface hints at potential enhancements with placeholder comments on features such as:

- Rule template management to standardize compatibility rules across parts.
- Copying compatibility rules between parts to ease rule creation.
- Querying for full part entities compatible with vehicle specs (instead of only IDs).

Some responsibilities originally intended for this service, like direct part querying, have been shifted to other services (e.g., `IPartService`), helping keep single responsibility principles intact.

## Summary

The `IPartCompatibilityRuleService` interface is a key component in managing vehicle-part compatibility within the AutoFusionPro system. By defining methods to create, modify, retrieve, and evaluate compatibility rules, this service underpins critical workflows for vehicle customization and parts recommendations, ensuring that users find correctly fitting parts efficiently and accurately.