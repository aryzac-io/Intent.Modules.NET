using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.PrivateSetters.Application.Common.Interfaces;
using CosmosDB.PrivateSetters.Domain.Common;
using CosmosDB.PrivateSetters.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBUnitOfWork", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence
{
    internal class CosmosDBUnitOfWork : ICosmosDBUnitOfWork
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _actions = new();
        private readonly ConcurrentDictionary<object, byte> _trackedEntities = new();
        private readonly IDomainEventService _domainEventService;

        public CosmosDBUnitOfWork(IDomainEventService domainEventService)
        {
            _domainEventService = domainEventService;
        }

        public void Track(object? entity)
        {
            if (entity is null)
            {
                return;
            }
            _trackedEntities.TryAdd(entity, default);
        }

        public void Enqueue(Func<CancellationToken, Task> action)
        {
            _actions.Enqueue(action);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchEvents(cancellationToken);
            while (_actions.TryDequeue(out var action))
            {
                await action(cancellationToken);
            }
        }

        private async Task DispatchEvents(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var domainEventEntity = _trackedEntities
                    .Keys
                    .OfType<IHasDomainEvent>()
                    .SelectMany(x => x.DomainEvents)
                    .FirstOrDefault(domainEvent => !domainEvent.IsPublished);

                if (domainEventEntity is null)
                {
                    break;
                }

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity, cancellationToken);
            }
        }
    }
}