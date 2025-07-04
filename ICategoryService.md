# ICategoryService Interface Documentation

**Location:** AutoFusionPro.Application/Interfaces/DataServices/ICategoryService.cs

## Overview

The `ICategoryService` interface defines a contract for managing product or part categories within the AutoFusionPro application. Categories play a crucial role in organizing components, parts, or products, and this service interface provides methods to create, read, update, and delete categories. Additionally, the interface supports hierarchical category structures, enabling parent-child relationships, which is important for representing complex category trees.

This interface is mainly designed for use by the application's business logic layer or UI components that need to manipulate or display category data.

## Interface Methods

### GetCategoryByIdAsync

**Purpose:** Retrieve detailed information about a single category by its unique identifier. Suitable for displaying details or editing an existing category.

**Parameters:**

- `id` (int): The unique ID of the desired category.

**Returns:** A `CategoryDto` containing the category information, or `null` if the category does not exist.

**Example usage:**

Fetch a category with ID 5 to display its details in an edit form.

### GetAllCategoriesHierarchicalAsync

**Purpose:** Retrieve all categories structured in a hierarchy, where each category may include nested subcategories. Ideal for admin panels or displays requiring the full tree.

**Parameters:**

- `onlyActive` (bool, default `true`): If true, returns only categories marked as active.

**Returns:** An enumerable collection of `CategoryDto` items arranged to reflect parent-child links.

**Example usage:**

Display all active categories in a tree view, where users can expand parent categories to see their children.

### GetTopLevelCategoriesAsync

**Purpose:** Retrieve categories that do not have a parent category, effectively the root nodes of the category hierarchy.

**Parameters:**

- `onlyActive` (bool, default `true`): Filters to only active top-level categories if true.

**Returns:** A collection of `CategoryDto` items representing the top-level categories.

**Example usage:**

Show top-level categories for quick navigation or category assignment.

### GetSubcategoriesAsync

**Purpose:** Get immediate child categories of a specified parent category.

**Parameters:**

- `parentCategoryId` (int): The ID of the parent category whose subcategories are requested.
- `onlyActive` (bool, default `true`): If true, returns only active subcategories.

**Returns:** An enumerable collection of `CategoryDto` representing the subcategories of the given parent.

**Example usage:**

Load subcategories on demand when a user expands a tree node for a specific category.

### GetAllCategoriesForSelectionAsync

**Purpose:** Provide a flat list of all categories for use in selection controls, such as dropdowns or combo boxes. Names in the list might be prefixed by their parent categories for clarity.

**Parameters:**

- `onlyActive` (bool, default `true`): Option to include only active categories.

**Returns:** An enumerable collection of `CategorySelectionDto`, each representing a category item formatted for selection.

**Example usage:**

Populating a dropdown menu where users select a category while adding a new part.

### CreateCategoryAsync

**Purpose:** Create a new category record in the system using provided input data.

**Parameters:**

- `createDto` (`CreateCategoryDto`): Data transfer object containing the new category details such as name, description, and possibly parent ID.

**Returns:** The newly created category information as a `CategoryDto`.

**Exceptions:**

- `FluentValidation.ValidationException` if the provided data fails validation rules.
- `AutoFusionPro.Core.Exceptions.ServiceException` in cases such as duplicate category names or database errors.

**Example usage:**

Adding a new category "Brakes" under the "Engine" parent category.

### UpdateCategoryAsync

**Purpose:** Modify the details of an existing category.

**Parameters:**

- `updateDto` (`UpdateCategoryDto`): Contains the ID of the category to update along with new values for its properties.

**Returns:** A `Task` that completes when the update operation finishes.

**Exceptions:**

- `FluentValidation.ValidationException` if validation of input fails.
- `AutoFusionPro.Core.Exceptions.ServiceException` if the category to update doesn't exist or other errors occur.

**Example usage:**

Changing the name or parent category of an existing category.

### DeleteCategoryAsync

**Purpose:** Remove a category from the system. Before deletion, the service checks for dependencies like subcategories or assigned parts to avoid data integrity issues.

**Parameters:**

- `id` (int): The ID of the category to be deleted.

**Returns:** A `Task` that completes upon successful deletion or cancellation due to dependencies.

**Exceptions:**

- `AutoFusionPro.Core.Exceptions.ServiceException` if the category does not exist or cannot be deleted because dependencies exist.

**Note:** The deletion may be implemented as a soft delete by marking the category inactive instead of physically removing it.

**Example usage:**

Deleting a discontinued category such as "Old Filters" after confirming no active parts rely on it.

### IsCategoryNameUniqueAsync

**Purpose:** Check if a category name is unique within a given parent category context, optionally excluding a particular category ID (useful during updates to ignore the current record).

**Parameters:**

- `name` (string): The name to validate for uniqueness.
- `parentCategoryId` (int? nullable): The parent category ID under which uniqueness should be checked, or null if checking among top-level categories.
- `excludeCategoryId` (int? nullable): An optional category ID to exclude from the uniqueness check, useful for ignoring the same category when updating.

**Returns:** `true` if the name is unique under the specified conditions; otherwise, `false`.

**Example usage:**

Ensuring "Filters" does not already exist under "Engine" before adding a new category.

## Additional Notes

The interface hints that a category entity may have an active/inactive flag to support soft deletion or filtering. There's a commented-out method idea for toggling the active status of categories, which could be implemented if needed.