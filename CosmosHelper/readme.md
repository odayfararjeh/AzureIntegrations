
# Cosmos Helper

This package contains custmoized code for azure cosmos CRUD operations. That is handling cosmos configurations and multiple operations on different containers.


### Dependencies
.Net 8
Microsoft.Azure.Cosmos (>= 3.52.0)

## Installation

Install this library with npm using nuget.org 

then

```bash
  PM> Install-Package OF.CosmosHelper -version 1.0.0
```


## Usage/Examples

Inject the using like the following for using `ICosmosOperations` interface.

```c#
using CosmosHelper.Interface;

```

And the following for using `CosmosOperations` Class.

```c#
using CosmosHelper.Class;

```

#### Instantiating directly in code:

```c#
var cosmosConfigurationDTO = new CosmosConfigurationDTO(connectionString, databaseName, containerName);

ICosmosOperations cosmosOperations = new CosmosOperations(cosmosConfigurationDTO );
```
The new update here is to allow the developer to create multiple databases instances at the same time, so you need only to set the record values and pass it to the artifact.

#### Instantiating using DI:

You need to register the service firstly in your startup

```c#
var cosmosConfigurationDTO = new CosmosConfigurationDTO(connectionString, databaseName, containerName);

services.AddScoped<ICosmosOperations>(s => new CosmosOperations(cosmosConfigurationDTO));
```

```
Note: In this version if you don't want to send the values using the configuration record then you can add values in your Environment Variables with these names

"CosmosDbConnectionString" : "Cosmos Connection string Value"

"DatabaseName" : "Cosmos Database Name Value"
 
"CosmosContainerName" : "Cosmos container Name Value"

then no need to send it to the helper, it'll be called by default by the artifact
```

```c#
Note:
If you want to register multiple instances with different cosmos instances or DBs, you can pass all of these information in a separated DTOs and register separated instances as the following:

services.AddScoped<ICosmosOperations, CosmosOperations>();
```

then in your controller in the constructor

```c#
private readonly ICosmosOperations _cosmosHelper
public Constructor(ICosmosOperations cosmosHelper)
{
    _cosmosHelper = cosmosHelper;
}
```

### Operations

- Get List

```c#
_cosmosHelper.GetItemsList<T>();
```

- Get List Async

```c#
_cosmosHelper.GetItemsListAsync<T>();
```

- Get Item By Predicate

```c#
_cosmosHelper.GetItemByPredicate<T, TOrder>("LINQ Search Query", "LINQ Order Query");
```

- Get Item By Predicate Async

```c#
_cosmosHelper.GetItemByPredicateAsync<T, TOrder>("LINQ Search Query", "LINQ Order Query");
```

- Get Items By Predicate Async

```c#
_cosmosHelper.GetItemsByPredicateAsync<T, TOrder, TDistinct>("LINQ Search Query", "LINQ Order Query", "LINQ Distinct Function");
```

- Get First Item

```c#
_cosmosHelper.GetFirstItem<T, TOrder>("LINQ Order Query");
```

- Get First Item Async

```c#
_cosmosHelper.GetFirstItemAsync<T, TOrder>("LINQ Order Query");
```

- Get Last Item

```c#
_cosmosHelper.GetLastItem<T, TOrder>("LINQ Order Query");
```

- Get Last Item Async

```c#
_cosmosHelper.GetLastItemAsync<T, TOrder>("LINQ Order Query");
```

- Upsert Item

```c#
_cosmosHelper.UpsertItemsAsync<T>(model, partiotion key);
```

- Upsert Bulk of Items with batching option

```c#
_cosmosHelper.UpsertItemsListAsync<T>(models, LINQ Partition Key Selector, batch size);
```

- Patch Item Async

```c#
_cosmosHelper.PatchItemsAsync<T>(string id, List<PatchOperation> operations, string partition Key);
```

- Delete Items Async

```c#
_cosmosHelper.DeleteItemsAsync<T>(string id, partiotion key);
```

- Despose Cosmos Client

```c#
_cosmosHelper.Dispose();
```

## Authors

- Odai Fararjeh - 2025


## Badges

[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)

