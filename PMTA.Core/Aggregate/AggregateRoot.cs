using PMTA.Core.Event;

namespace PMTA.Core.Aggregate
{
    public abstract class AggregateRoot
    {
        protected Guid _id;
        private readonly List<BaseEvent> _events = new();
        public Guid Id { get => _id; }
        public int Version { get; set; } = -1;

        public List<BaseEvent> GetUncommittedChanges()
        {
            return _events;
        }

        public void MarkChangesAsCommited()
        {
            _events.Clear();
        }

        private void ApplyChange(BaseEvent @event, bool isNewEvent)
        {
            var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });
            if (method == null)
            {
                throw new ArgumentException(nameof(method), $"The Apply method was not found in the aggregate for the event: {@event.GetType().Name}");
            }

            method.Invoke(this, new object[] { @event });
            if (isNewEvent)
            {
                _events.Add(@event);
            }
        }

        protected void RaiseEvent(BaseEvent @event)
        {
            ApplyChange(@event, true);
        }

        public void ReplayEvents(List<BaseEvent> events)
        {
            foreach (var @event in events)
            {
                ApplyChange(@event, false);
            }
        }
    }
}
