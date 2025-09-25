using Microsoft.Azure.Cosmos;
using System.Linq.Expressions;

namespace CosmosHelper.IOC
{
    public interface ICosmosOperations
    {
        List<T> GetItemsList<T>();
        Task<List<T>> GetItemsListAsync<T>(CancellationToken cancellationToken = default);
        T GetItemByPredicate<T>(Expression<Func<T, bool>> keySelector);
        T GetItemByPredicate<T, TOrder>(Expression<Func<T, bool>> keySelector, Expression<Func<T, TOrder>> sortCondition);
        Task<T> GetItemByPredicateAsync<T>(Expression<Func<T, bool>> keySelector, CancellationToken cancellationToken = default);
        Task<T> GetItemByPredicateAsync<T, TOrder>(Expression<Func<T, bool>> keySelector, Expression<Func<T, TOrder>> sortCondition, CancellationToken cancellationToken = default);
        Task<List<T>> GetItemsByPredicateAsync<T>(Expression<Func<T, bool>> keySelector, CancellationToken cancellationToken = default);
        Task<List<T>> GetItemsByPredicateAsync<T, TOrder>(Expression<Func<T, bool>> keySelector, Expression<Func<T, TOrder>> sortCondition, CancellationToken cancellationToken = default);
        Task<List<T>> GetItemsByPredicateAsync<T, TOrder, TDistinct>(Expression<Func<T, bool>> keySelector, Expression<Func<T, TOrder>> sortCondition, Func<T, TDistinct> distinctCondition, CancellationToken cancellationToken = default);
        T GetLastItem<T, TOrder>(Expression<Func<T, TOrder>> sortCondition);
        Task<T> GetLastItemAsync<T, TOrder>(Expression<Func<T, TOrder>> sortCondition, CancellationToken cancellationToken = default);
        T GetFirstItem<T, TOrder>(Expression<Func<T, TOrder>> sortCondition);
        Task<T> GetFirstItemAsync<T, TOrder>(Expression<Func<T, TOrder>> sortCondition, CancellationToken cancellationToken = default);
        Task<ItemResponse<T>> UpsertItemAsync<T>(T item, string partitionKey);
        Task<ItemResponse<T>> PatchItemOperationsAsync<T>(string id, List<PatchOperation> operations, string partitionKey);
        Task<ItemResponse<T>> DeleteItemAsync<T>(string id, string partitionKey);
        Task<List<ItemResponse<T>>> UpsertItemsListAsync<T>(List<T> items, Expression<Func<T, string>> partitionKeySelector, int? batchSize = null);
        void Dispose();
    }
}
