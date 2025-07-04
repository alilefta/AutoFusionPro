# Documentation: IPasswordHashingService Interface

File Path: `AutoFusionPro.Application/Interfaces/DataServices/IPasswordHashingService.cs`

## Overview

The **IPasswordHashingService** interface defines a contract for password hashing services, which are essential for securely handling user passwords within the AutoFusionPro system. This interface provides methods that allow converting plain-text passwords into secure hashed formats along with accompanying salts and verifying user passwords during authentication.

Using this interface ensures that any concrete implementation will support consistent methods for hashing and verifying passwords, which helps improve security by protecting user credentials against unauthorized access.

## Interface Details

### Method: HashPassword

**Purpose:** This asynchronous method accepts a plain-text password and returns a tuple containing the secure hash of the password along with a cryptographic salt. The salt is a random value added to the password before hashing to enhance security by protecting against rainbow table attacks.

**Parameters:**

- `password` (string): The user's plain-text password that needs to be hashed.

**Returns:**

- A `Task` that resolves to a tuple with two string elements:
    - `hash`: The derived password hash.
    - `salt`: The generated salt used during the hashing process.

**Usage Example:**

```

var (hash, salt) = await passwordHashingService.HashPassword("mySecretPassword123");

```

This method is crucial when a user creates or updates their password, ensuring the application stores only hashed passwords and never retains plain-text passwords.

### Method: VerifyPassword

**Purpose:** This method verifies if a given plain-text password matches a previously stored password hash and salt. It is used during the login process to authenticate users.

**Parameters:**

- `password` (string): The plain-text password to check.
- `hash` (string): The stored hash to compare against.
- `salt` (string): The salt originally used to hash the stored password.

**Returns:** A boolean value:

- `true` if the password matches the stored hash and salt combination.
- `false` if the verification fails.

**Usage Example:**

```

bool isValid = passwordHashingService.VerifyPassword("candidatePassword", storedHash, storedSalt);
if (isValid) {
    // Proceed with login
} else {
    // Reject login attempt
}

```

This method is essential to confirm a user's identity during authentication while ensuring that plain-text passwords are never stored or exposed.

## Summary

The `IPasswordHashingService` interface encapsulates essential methods to safely hash passwords and verify them later for authentication purposes. Following this contract helps maintain robust security practices for user password management in the AutoFusionPro application.