
# Keyvault Helper

This package contains custmoized code for Key Vault operations.


### Dependencies
.Net 8


## Configuration

Add the following as part of your configurations file with it's value
"KEY_VAULT_NAME"

## Installation

Install this library with npm nuget.org

then

```bash
  PM> Install-Package OF.KeyVaultHelper -version 1.0.0
```


## Usage/Examples

Inject the using like the following for using `IKeyVaultOperations` interface.

```c#
using KeyVaultHelper.Interface;

```

And the following for using `KeyVaultOperations` Class.

```c#
using KeyVaultHelper.Class;

```

#### Instantiating directly in code:

```c#
IKeyVaultOperations _keyVaultOperations = new KeyVaultOperations();
```
The reason behind allow the user to send the container name to give the ability to create multiple connections to multiple containers in the same database.

#### Instantiating using DI:

You need to register the service firstly in your startup

```c#
services.AddScoped<IKeyVaultOperations, KeyVaultOperations>();
```

then in your controller in the constructor

```c#
private readonly IKeyVaultOperations _keyVaultOperations 
public Constructor(IKeyVaultOperations keyVaultOperations )
{
    _keyVaultOperations = keyVaultOperations ;
}
```

### Operations

- Get Key Vault Secret

```c#
_keyVaultOperations.GetSecretAsync(secretname);
```

- Get Key Vault Secrets List

```c#
_keyVaultOperations.GetSecretsAsync(Dictionary<string, string> keyValues);
```


## Authors

- Odai Fararjeh - 2025


## Badges

[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)

