using CQRS.Core.Domain;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastracture;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers
{
    public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
    {
        private readonly IEventStore _eventStore;
        public EventSourcingHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }
        public async Task<PostAggregate> GetByIdAsync(Guid id)
        {
            var aggregate = new PostAggregate();
            var events = await _eventStore.GetEventsAsync(id);
            if (events == null || !events.Any())
            {
                return aggregate;
            }
            aggregate.ReplayEvents(events);
            aggregate.Version = events.Max(e => e.Version);

            return aggregate;
        }

        public async Task SaveAsync(AggregateRoot aggregate)
        {
            await _eventStore.SaveEventAsync(aggregate.Id, aggregate.GetUncommittedChanges(), aggregate.Version);
            aggregate.MarkChangesAsCommitted();
        }
    }
}