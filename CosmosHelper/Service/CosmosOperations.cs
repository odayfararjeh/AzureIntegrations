using CosmosHelper.Domain.DTO;
using CosmosHelper.Helper;
using CosmosHelper.IOC;
using Microsoft.Azure.Cosmos;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace CosmosHelper.Services
{
    public class CosmosOperations : ICosmosOperations
    {
        private readonly CosmosClientOptions _cosmosClientOptions;
        private readonly CosmosClient _cosmosClient;
        private readonly Container _cosmosContainer;
        private readonly CosmosInternalOperations _cosmosInternalOperations;

        public CosmosOperations(CosmosConfigurationDTO cosmosConfiguration = null)
        {
            _cosmosClientOptions = new CosmosClientOptions
            {
                AllowBulkExecution = true,
                ConnectionMode = ConnectionMode.Gateway
            };

            _cosmosClient = new CosmosClient(cosmosConfiguration?.CosmosConnectionString ?? Environment.GetEnvironmentVariable("CosmosDbConnectionString"), _cosmosClientOptions);

            if (_cosmosClient is null)
                throw new InvalidOperationException("Cosmos Connection String is invalid or missing");

            _cosmosContainer = _cosmosClient.GetContainer(
                cosmosConfiguration?.CosmosDatabaseName ?? Environment.GetEnvironmentVariable("DatabaseName"),
                cosmosConfiguration?.CosmosContainerName ?? Environment.GetEnvironmentVariable("CosmosContainerName")
                );

            if (_cosmosContainer is null)
                throw new InvalidOperationException("Cosmos Container Configurations are invalid or missing");

            _cosmosInternalOperations = new CosmosInternalOperations(_cosmosContainer);
        }

        public List<T> GetItemsList<T>()
        {
            try
            {
                return _cosmosContainer.GetItemLinqQueryable<T>(true).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<T>> GetItemsListAsync<T>(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _cosmosInternalOperations.ExecuteQueryAsync<T, object>(null, null, cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public T GetItemByPredicate<T>(Expression<Func<T, bool>> keySelector)
        {
            try
            {
                return _cosmosContainer.GetItemLinqQueryable<T>(true)
                       .Where(keySelector)
                       .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public T GetItemByPredicate<T, TOrder>(Expression<Func<T, bool>> keySelector, Expression<Func<T, TOrder>> sortCondition)
        {
            try
            {
                return _cosmosContainer.GetItemLinqQueryable<T>(true)
                       .Where(keySelector)
                       .OrderByDescending(sortCondition)
                       .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<T> GetItemByPredicateAsync<T>(Expression<Func<T, bool>> keySelector, CancellationToken cancellationToken = default)
        {
            try
            {
                List<T> results = await _cosmosInternalOperations.ExecuteQueryAsync<T, object>(keySelector, null, cancellationToken);

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<T> GetItemByPredicateAsync<T, TOrder>(Expression<Func<T, bool>> keySelector, Expression<Func<T, TOrder>> sortCondition, CancellationToken cancellationToken = default)
        {
            try
            {
                List<T> results = await _cosmosInternalOperations.ExecuteQueryAsync(keySelector, sortCondition, cancellationToken);

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<T>> GetItemsByPredicateAsync<T>(Expression<Func<T, bool>> keySelector, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _cosmosInternalOperations.ExecuteQueryAsync<T, object>(keySelector, null, cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<T>> GetItemsByPredicateAsync<T, TOrder>(Expression<Func<T, bool>> keySelector, Expression<Func<T, TOrder>> sortCondition, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _cosmosInternalOperations.ExecuteQueryAsync(keySelector, sortCondition, cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<T>> GetItemsByPredicateAsync<T, TOrder, TDistinct>(Expression<Func<T, bool>> keySelector, Expression<Func<T, TOrder>> sortCondition, Func<T, TDistinct> distinctCondition, CancellationToken cancellationToken = default)
        {
            try
            {
                List<T> resultList = await _cosmosInternalOperations.ExecuteQueryAsync(keySelector, sortCondition, cancellationToken);
                return resultList.DistinctBy(distinctCondition).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public T GetLastItem<T, TOrder>(Expression<Func<T, TOrder>> sortCondition)
        {
            try
            {
                return _cosmosContainer.GetItemLinqQueryable<T>(true)
                    .OrderByDescending(sortCondition)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<T> GetLastItemAsync<T, TOrder>(Expression<Func<T, TOrder>> sortCondition, CancellationToken cancellationToken = default)
        {
            try
            {
                List<T> resultList = await _cosmosInternalOperations.ExecuteQueryAsync<T, TOrder>(null, sortCondition, cancellationToken);

                return resultList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public T GetFirstItem<T, TOrder>(Expression<Func<T, TOrder>> sortCondition)
        {
            try
            {
                return _cosmosContainer.GetItemLinqQueryable<T>(true)
                    .OrderBy(sortCondition)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<T> GetFirstItemAsync<T, TOrder>(Expression<Func<T, TOrder>> sortCondition, CancellationToken cancellationToken = default)
        {
            try
            {
                List<T> resultList = await _cosmosInternalOperations.ExecuteQueryAsync<T, TOrder>(null, sortCondition, cancellationToken, false);

                return resultList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ItemResponse<T>> UpsertItemAsync<T>(T item, string partitionKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(partitionKey))
                    throw new ArgumentNullException(nameof(partitionKey), "Partition Key is missing or empty");

                return await _cosmosContainer.UpsertItemAsync(item, new PartitionKey(partitionKey));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ItemResponse<T>> PatchItemOperationsAsync<T>(string id, List<PatchOperation> operations, string partitionKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(partitionKey))
                    throw new ArgumentNullException(nameof(partitionKey), "Partition Key is missing or empty");

                return await _cosmosContainer.PatchItemAsync<T>(id, new PartitionKey(partitionKey), operations);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ItemResponse<T>> DeleteItemAsync<T>(string id, string partitionKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(partitionKey))
                    throw new ArgumentNullException(nameof(partitionKey), "Partition Key is missing or empty");

                return await _cosmosContainer.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<ItemResponse<T>>> UpsertItemsListAsync<T>(List<T> items, Expression<Func<T, string>> partitionKeySelector, int? batchSize = null)
        {
            try
            {
                if (partitionKeySelector is null)
                    throw new ArgumentNullException(nameof(partitionKeySelector), "Partition Key Predicate is missing or empty");

                Func<T, string> GetPartitionKey = partitionKeySelector.Compile();

                ConcurrentQueue<Exception> exceptions = new();
                batchSize ??= items.Count();
                var results = new List<ItemResponse<T>>();
                foreach (IEnumerable<T> itemsBatches in ExtensionHelper.Batch(items, (int)batchSize))
                {
                    var tasks = new List<Task<ItemResponse<T>>>();
                    foreach (T item in itemsBatches)
                    {
                        try
                        {
                            string partitionKeyValue = GetPartitionKey(item);
                            if (string.IsNullOrWhiteSpace(partitionKeyValue))
                                throw new ArgumentNullException(nameof(partitionKeyValue), "Partition key is missing or empty for one or more items.");

                            Task<ItemResponse<T>> task = _cosmosContainer.UpsertItemAsync<T>(item, new PartitionKey(partitionKeyValue));

                            tasks.Add(_cosmosInternalOperations.ExecuteWithRetriesAsync((() => task)));
                        }
                        catch (Exception ex)
                        {
                            exceptions.Enqueue(ex);
                        }
                    }

                    results.AddRange(await Task.WhenAll(tasks));
                }

                if (!exceptions.IsEmpty)
                {
                    throw new AggregateException(exceptions);
                }

                return results;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Dispose() => _cosmosClient.Dispose();
    }
}
