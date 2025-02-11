using System;
using System.Threading.Tasks;
using System.Transactions;
using CleanArchitecture.SingleFiles.Application.Common.Eventing;
using CleanArchitecture.SingleFiles.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.WrapperConsumer", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Infrastructure.Eventing
{
    public class WrapperConsumer<THandler, TMessage> : IConsumer<TMessage>
        where TMessage : class
        where THandler : IIntegrationEventHandler<TMessage>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICosmosDBUnitOfWork _cosmosDBUnitOfWork;
        private readonly IDaprStateStoreUnitOfWork _daprStateStoreUnitOfWork;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMongoDbUnitOfWork _mongoDbUnitOfWork;

        public WrapperConsumer(IServiceProvider serviceProvider,
            ICosmosDBUnitOfWork cosmosDBUnitOfWork,
            IDaprStateStoreUnitOfWork daprStateStoreUnitOfWork,
            IUnitOfWork unitOfWork,
            IMongoDbUnitOfWork mongoDbUnitOfWork)
        {
            _serviceProvider = serviceProvider;
            _cosmosDBUnitOfWork = cosmosDBUnitOfWork;
            _daprStateStoreUnitOfWork = daprStateStoreUnitOfWork;
            _unitOfWork = unitOfWork;
            _mongoDbUnitOfWork = mongoDbUnitOfWork;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var eventBus = _serviceProvider.GetService<MassTransitEventBus>()!;
            eventBus.ConsumeContext = context;
            var handler = _serviceProvider.GetService<THandler>()!;

            // The execution is wrapped in a transaction scope to ensure that if any other
            // SaveChanges calls to the data source (e.g. EF Core) are called, that they are
            // transacted atomically. The isolation is set to ReadCommitted by default (i.e. read-
            // locks are released, while write-locks are maintained for the duration of the
            // transaction). Learn more on this approach for EF Core:
            // https://docs.microsoft.com/en-us/ef/core/saving/transactions#using-systemtransactions
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await handler.HandleAsync(context.Message, context.CancellationToken);

                // By calling SaveChanges at the last point in the transaction ensures that write-
                // locks in the database are created and then released as quickly as possible. This
                // helps optimize the application to handle a higher degree of concurrency.
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);

                // Commit transaction if everything succeeds, transaction will auto-rollback when
                // disposed if anything failed.
                transaction.Complete();
            }

            await _cosmosDBUnitOfWork.SaveChangesAsync(context.CancellationToken);
            await _daprStateStoreUnitOfWork.SaveChangesAsync(context.CancellationToken);
            await _mongoDbUnitOfWork.SaveChangesAsync(context.CancellationToken);
            await eventBus.FlushAllAsync(context.CancellationToken);
        }
    }

    public class WrapperConsumerDefinition<THandler, TMessage> : ConsumerDefinition<WrapperConsumer<THandler, TMessage>>
        where TMessage : class
        where THandler : IIntegrationEventHandler<TMessage>
    {
        private readonly IServiceProvider _serviceProvider;

        public WrapperConsumerDefinition(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<WrapperConsumer<THandler, TMessage>> consumerConfigurator)
        {
            endpointConfigurator.UseInMemoryInboxOutbox(_serviceProvider);
        }
    }
}