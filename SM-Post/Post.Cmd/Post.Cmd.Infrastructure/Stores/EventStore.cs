using CQRS.Core.Infrastracture;
using CQRS.Core.Events;
using CQRS.Core.Domain;
using CQRS.Core.Exceptions;
using Post.Cmd.Domain.Aggregates;
using CQRS.Core.Producer;

namespace Post.Cmd.Infrastructure.Stores
{
    public class EventStore: IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IEventProducer _eventProducer;
        public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
        {
            _eventStoreRepository = eventStoreRepository;
            _eventProducer = eventProducer;
        }
        public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
              var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
            if (expectedVersion != 1 && eventStream[^1].Version != expectedVersion)
            {
               throw new ConcurrencyException("Concurrency violation. One or more events already been applied to aggregate.");
            }
            var version = expectedVersion;
            foreach (var @event in events)
            {
                @event.Version = ++version;
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel
                {
                   Timestamp = DateTime.UtcNow,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(PostAggregate),
                    Version = version,
                    EventType = eventType,
                    EventData = @event
                };
                await _eventStoreRepository.SaveAsync(eventModel);
                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                await _eventProducer.ProduceAsync(topic!, @event);
            }
        }

        public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
            if (eventStream == null || !eventStream.Any())
            {
               throw new AggregateNotFoundException("Incorrect post ID provided.");
            }
            return eventStream.OrderBy(e => e.Version).Select(e => e.EventData!).ToList();
        }
    }
}