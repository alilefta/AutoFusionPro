# AutoFusionPro Documentation

## AutoFusionPro.Application.Commands

# Documentation for RelayCommand Class

**File Location:** AutoFusionPro.Application/Commands/RelayCommand.cs

## Overview

The `RelayCommand` class is a reusable implementation of the `ICommand` interface in WPF (Windows Presentation Foundation). It helps developers connect user interface controls, such as buttons, to code logic in a clean and decoupled manner. This class allows command logic and enabling conditions to be defined via delegates, making it very flexible and easy to use.

## Namespace

The class is part of the `AutoFusionPro.Application.Commands` namespace.

## Class: RelayCommand

`RelayCommand` implements the `ICommand` interface, which means it must provide functionality for executing commands and checking if the command can execute. It allows you to define what happens when the command is called and under what conditions the command is enabled.

### Private Members

- **_action**: A delegate of type `Action<object>` that holds the logic to be executed when the command runs.
- **_canExecute**: A delegate of type `Predicate<object>` that determines whether the command is allowed to execute based on a condition.

These encapsulate the core logic and control for the command behavior.

### Events

`CanExecuteChanged`

An event that notifies the system when the ability to execute the command may have changed. For example, this affects if a button is enabled or disabled. It hooks into WPF's `CommandManager.RequerySuggested` event to automatically refresh control states.

### Constructor

The constructor requires two parameters:

- `action`: The code to execute when the command runs. Cannot be null.
- `canExecute`: The condition under which the command is enabled. Cannot be null.

Passing null for either will throw an `ArgumentNullException`.

**Example:**

```
var saveCommand = new RelayCommand(
    action: obj => SaveData(),
    canExecute: obj => CanSave()
);

```

Here, `SaveData` is the action performed, and `CanSave` determines if the Save button is enabled.

### Core Methods

`CanExecute(object? parameter)`

Returns a boolean indicating if the command can be run at the moment. It calls the `_canExecute` predicate with the optional parameter. This method enables UI elements like buttons to be disabled when the command shouldn't run.

`Execute(object? parameter)`

Runs the command logic by invoking the stored `_action` delegate, optionally using the provided parameter.

`RaiseCanExecuteChanged()`

Allows manual notification that the ability of the command to execute has changed. This triggers WPF to reevaluate commands and update UI controls accordingly.

### Usage Example

Suppose you want a button to save a form only when the form is valid. You might write:

```
// Define the command with logic and condition
RelayCommand saveCommand = new RelayCommand(
    action: param => SaveForm(),                     // Action to execute
    canExecute: param => IsFormValid()               // Condition to enable button
);

// Bind saveCommand to a button's Command property in your UI

```

When `IsFormValid()` returns false, the button will be disabled automatically, thanks to `RelayCommand`.