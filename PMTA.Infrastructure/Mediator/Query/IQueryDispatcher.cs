﻿using PMTA.Core.Query;

namespace PMTA.Infrastructure.Mediator.Query
{
    public interface IQueryDispatcher<TEntity>
    {
        void RegisterHandler<TQuery>(Func<TQuery, Task<List<TEntity>>> handler) where TQuery:BaseQuery;
        Task<List<TEntity>> SendAsync(BaseQuery query);
    }
}
