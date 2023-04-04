using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PMTA.Domain.Event;

namespace PMTA.Infrastructure.Repository
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly IMongoCollection<EventModel> _eventStoreCollection;
        public EventStoreRepository(IOptions<MongoDBConfig> config)
        {
            var mongoClient = new MongoClient(config.Value.ConnectionString);
            var mongoDb = mongoClient.GetDatabase(config.Value.Database);

            _eventStoreCollection = mongoDb.GetCollection<EventModel>(config.Value.CollectionName);
        }
        public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
        {
            return await _eventStoreCollection.Find(c => c.AggregateIdentifier == aggregateId)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task SaveAsync(EventModel @event)
        {
            await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
        }
    }
}
