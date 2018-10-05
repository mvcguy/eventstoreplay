using System;
using expense.web.api.Values.Aggregate.Events.Base;
using expense.web.api.Values.Aggregate.Events.Root;
using expense.web.api.Values.Aggregate.Model;
using expense.web.api.Values.Aggregate.Repository;
using expense.web.eventstore.EventStoreDataContext;

namespace expense.web.api.Values.Aggregate
{
    public class ValuesRootAggregate : AggregateBase<EventModel>, IValuesRootAggregateModel
    {

        // Notes:
        // 1. When we construct the aggregate, we don't wanna publish event for each property changes, rather an event called 'ValueCreatedEvent' is fired containing all required props
        // 2. We want to publish individual events for each property change. e.g., NameChangedEvent if the value of the ValueAggregate is changed.

        private readonly IRepository<ValuesRootAggregate> _rootAggregateRepository;

        public int TenantId { get; private set; }

        public string Code { get; private set; }

        public string Name { get; private set; }

        public string Value { get; private set; }

        public Guid CommitId { get; set; }

        // this constructor is used in generic reconstruction of this class by repositories
        public ValuesRootAggregate()
        {

        }

        public ValuesRootAggregate(IRepository<ValuesRootAggregate> rootAggregateRepository) : this(Guid.NewGuid(), -1, rootAggregateRepository)
        {

        }

        public ValuesRootAggregate(Guid id, long version, IRepository<ValuesRootAggregate> rootAggregateRepository)
        {
            this.Id = id;
            this.Version = version;
            _rootAggregateRepository = rootAggregateRepository;
            this.CommitId = Guid.NewGuid();
        }

        public bool Save()
        {
            return _rootAggregateRepository.Save(this);
        }

        public void CreateValue(IValuesRootAggregateModel model)
        {
            // Note: We want to modify the props using there methods, to keep the business logic in one place
            // Also we don't want to fire individual prop changed events when an aggregate is created first time
            // Only one event called ValueCreated will be fired/applied

            ChangeTenantId(model.TenantId);
            ChangeName(model.Name, applyEvent: false);
            ChangeCode(model.Code, applyEvent: false);
            ChangeValue(model.Value, applyEvent: false);

            ApplyEvent(ValueEventType.ValueCreated);
        }

        public void ChangeTenantId(int tenantId)
        {
            // Note: We don't allow tenantId to be changed, except for when an aggregate is created first time or 
            // Reconstructed from events (Replay events)
            ThrowIfNullOrNegative(tenantId);
            this.TenantId = tenantId;
        }

        public void ChangeCode(string code, bool applyEvent = true)
        {
            ThrowIfNullOrEmpty(code);
            this.Code = code;

            if (!applyEvent) return;
            this.ApplyEvent(ValueEventType.CodeChanged);
        }

        public void ChangeName(string name, bool applyEvent = true)
        {
            ThrowIfNullOrEmpty(name);
            this.Name = name;

            if (!applyEvent) return;
            this.ApplyEvent(ValueEventType.NameChanged);
        }

        public void ChangeValue(string value, bool applyEvent = true)
        {
            ThrowIfNullOrEmpty(value);
            this.Value = value;

            if (!applyEvent) return;
            this.ApplyEvent(ValueEventType.ValueChanged);
        }

        private void ThrowIfNullOrEmpty(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException();
        }

        private void ThrowIfNullOrNegative(int value)
        {
            if (value <= 0)
                throw new ArgumentNullException();
        }

        /// <summary>
        /// Apply event performs two operations
        /// 1. Adds the event to the list of uncommitted events
        /// 2. Increment the version of the aggregate
        /// </summary>
        /// <param name="eventType"></param>
        public void ApplyEvent(ValueEventType eventType)
        {
            switch (eventType)
            {
                case ValueEventType.CodeChanged:
                    base.ApplyEvent(new CodeChangedEvent(this));
                    break;
                case ValueEventType.NameChanged:
                    base.ApplyEvent(new NameChangedEvent(this));
                    break;
                case ValueEventType.ValueChanged:
                    base.ApplyEvent(new ValueChangedEvent(this));
                    break;
                case ValueEventType.ValueCreated:
                    base.ApplyEvent(new ValueCreatedEvent(this));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
            }
        }

        #region ReconstructAggregate

        public void Handle(ValueCreatedEvent @event)
        {
            CheckEvent(@event);

            this.Code = @event.Code;
            this.Name = @event.Name;
            this.TenantId = @event.TenantId;
            this.Value = @event.Value;
        }

        public void Handle(NameChangedEvent @event)
        {
            CheckEvent(@event);
            this.Name = @event.Name;
        }

        public void Handle(ValueChangedEvent @event)
        {
            CheckEvent(@event);
            this.Value = @event.Value;
        }

        public void Handle(CodeChangedEvent @event)
        {
            CheckEvent(@event);
            this.Code = @event.Code;

        }

        public void CheckEvent(EventBase @event)
        {
            if (this.Id != @event.Id) throw new InvalidOperationException();
        }

        #endregion

    }
}
