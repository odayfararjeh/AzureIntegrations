using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Polly;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;

namespace CosmosHelper.Helper
{
    public class CosmosInternalOperations
    {
        private readonly Container _cosmosContainer;
        public CosmosInternalOperations(Container cosmosContainer)
        {
            _cosmosContainer = cosmosContainer ?? throw new ArgumentNullException(nameof(cosmosContainer));
        }

        public async Task<List<T>> ExecuteQueryAsync<T, TOrder>(
           Expression<Func<T, bool>> keySelector,
           Expression<Func<T, TOrder>> sortCondition,
           CancellationToken cancellationToken,
           bool isDescending = true)
        {
            IAsyncEnumerable<T> results;

            if (sortCondition is not null)
            {
                Expression<Func<T, bool>> filter = keySelector ?? (_ => true);

                results = isDescending
                    ? QueryDescendingAsync(filter, sortCondition, cancellationToken)
                    : QueryAscendingAsync(filter, sortCondition, cancellationToken);
            }
            else if (keySelector is not null)
            {
                results = QueryAsync<T>(keySelector, cancellationToken);
            }
            else
            {
                results = QueryAsync<T>(cancellationToken);
            }

            List<T> resultList = new();
            await foreach (var item in results)
            {
                resultList.Add(item);
            }

            return resultList;
        }
        public async Task<T> ExecuteWithRetriesAsync<T>(Func<Task<T>> taskQuery)
        {
            return await Policy
                    .Handle<CosmosException>(ex => ex.StatusCode == HttpStatusCode.TooManyRequests)
                    .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(2))
                    .ExecuteAsync(taskQuery);
        }

        public async IAsyncEnumerable<T> QueryAsync<T>([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            FeedIterator<T> query = _cosmosContainer.GetItemLinqQueryable<T>(true)
                   .ToFeedIterator();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync(cancellationToken);
                foreach (var item in response)
                    yield return item;
            }
        }

        public async IAsyncEnumerable<T> QueryAsync<T>(Expression<Func<T, bool>> keySelector,
                                                       [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            FeedIterator<T> query = _cosmosContainer.GetItemLinqQueryable<T>(true)
                   .Where(keySelector)
                   .ToFeedIterator();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync(cancellationToken);
                foreach (var item in response)
                    yield return item;
            }
        }

        public async IAsyncEnumerable<T> QueryDescendingAsync<T, TOrder>(Expression<Func<T, bool>> keySelector,
                                                                         Expression<Func<T, TOrder>> sortCondition,
                                                                         [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            FeedIterator<T> query = _cosmosContainer.GetItemLinqQueryable<T>(true)
                   .Where(keySelector)
                   .OrderByDescending(sortCondition)
                   .ToFeedIterator();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync(cancellationToken);
                foreach (var item in response)
                    yield return item;
            }
        }

        public async IAsyncEnumerable<T> QueryAscendingAsync<T, TOrder>(Expression<Func<T, bool>> keySelector,
                                                                        Expression<Func<T, TOrder>> sortCondition,
                                                                        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            FeedIterator<T> query = _cosmosContainer.GetItemLinqQueryable<T>(true)
                   .Where(keySelector)
                   .OrderBy(sortCondition)
                   .ToFeedIterator();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync(cancellationToken);
                foreach (var item in response)
                    yield return item;
            }
        }
    }
}
