# Documentation for `IAuthenticationService` Interface

This documentation provides an overview and detailed explanation of the `IAuthenticationService` interface located in the `AutoFusionPro.Application.Interfaces.Authentication` namespace. This interface plays a key role in handling user authentication within the AutoFusionPro application.

## Overview

The `IAuthenticationService` interface defines the core contract that any authentication service implementation should follow in the AutoFusionPro application. It facilitates user login, logout, and notifies subscribers about changes in the authentication state.

By implementing this interface, developers ensure a consistent way for the app to authenticate users and respond to authentication status changes.

## Members of `IAuthenticationService`

### AuthenticationChanged Event

**Type:** `EventHandler`

This event is triggered whenever the authentication state changes, such as when a user logs in or logs out.

Subscribers can listen to this event to update UI elements or perform other actions that depend on whether a user is authenticated.

*Example:*

```
authenticationService.AuthenticationChanged += (sender, args) =>
{
    // Update UI or refresh data when authentication changes
    Console.WriteLine("Authentication state has changed.");
};
```

### AuthenticateAsync Method

**Signature:** `Task<bool> AuthenticateAsync(string username, string password)`

This asynchronous method attempts to authenticate a user based on the provided username and password.

It returns a `Task` that resolves to a boolean indicating whether the authentication was successful (`true`) or not (`false`).

*Usage example:*

```
bool isAuthenticated = await authenticationService.AuthenticateAsync("user123", "password");
if (isAuthenticated)
{
    Console.WriteLine("Welcome back, user123!");
}
else
{
    Console.WriteLine("Authentication failed. Please check your credentials.");
}
```

### Logout Method

**Signature:** `void Logout()`

This method logs out the currently authenticated user, effectively ending their authenticated session.

Calling this method should also trigger the `AuthenticationChanged` event so that subscribed components can respond appropriately.

*Example:*

```
authenticationService.Logout();
// After this, UI can refresh to show logged-out state

```

## Potential Future Members (Comments)

The interface shows commented suggestions for potential future additions to enhance usability:

- `Task<bool> IsUserLoggedInAsync();` - An asynchronous method that might check if a user is currently logged in.
- `Task<UserDto?> GetCurrentUserAsync();` - An asynchronous method that could fetch the current user's details, assuming a `SessionManager` or similar exposes user information.

These methods would add convenience for retrieving user session information but are not part of the current contract.

## Summary

To sum up, the `IAuthenticationService` interface provides a simple yet effective mechanism to:

- Authenticate users asynchronously
- Handle logout functionality
- Notify interested components when the authentication state changes

This interface serves as a foundation for implementing authentication logic in the AutoFusionPro system, helping maintain separation of concerns and making authentication mechanisms easily interchangeable and testable.