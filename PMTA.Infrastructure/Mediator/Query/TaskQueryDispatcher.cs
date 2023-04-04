using PMTA.Core.Query;
using PMTA.Domain.Entity;

namespace PMTA.Infrastructure.Mediator.Query
{
    public class TaskQueryDispatcher : IQueryDispatcher<TaskEntity>
    {
        private readonly Dictionary<Type, Func<BaseQuery, Task<List<TaskEntity>>>> _handlers = new();
        public void RegisterHandler<TQuery>(Func<TQuery, Task<List<TaskEntity>>> handler) where TQuery : BaseQuery
        {
            if (_handlers.ContainsKey(typeof(TQuery)))
            {
                throw new ArgumentOutOfRangeException("You cannot register the same query handle twice.");
            }

            _handlers.Add(typeof(TQuery), query => handler((TQuery)query));
        }

        public async Task<List<TaskEntity>> SendAsync(BaseQuery query)
        {
            if (_handlers.TryGetValue(query.GetType(), out Func<BaseQuery, Task<List<TaskEntity>>> handler))
            {
                return await handler(query);
            }
            throw new ArgumentException(nameof(handler), "No query handler was registered.");
        }
    }
}
