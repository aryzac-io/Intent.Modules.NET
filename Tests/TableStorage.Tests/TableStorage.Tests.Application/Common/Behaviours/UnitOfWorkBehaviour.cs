using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TableStorage.Tests.Application.Common.Interfaces;
using TableStorage.Tests.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.UnitOfWorkBehaviour", Version = "1.0")]

namespace TableStorage.Tests.Application.Common.Behaviours
{
    /// <summary>
    /// Ensures that all operations processed as part of handling a <see cref="ICommand"/> either
    /// pass or fail as one unit. This behaviour makes it unnecessary for developers to call
    /// SaveChangesAsync() inside their business logic (e.g. command handlers), and doing so should
    /// be avoided unless absolutely necessary.
    /// </summary>
    public class UnitOfWorkBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, ICommand
    {
        private readonly IUnitOfWork _dataSource;
        private readonly ITableStorageUnitOfWork _tableStorageDataSource;

        public UnitOfWorkBehaviour(IUnitOfWork dataSource, ITableStorageUnitOfWork tableStorageDataSource)
        {
            _dataSource = dataSource;
            _tableStorageDataSource = tableStorageDataSource;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            TResponse response;

            // The execution is wrapped in a transaction scope to ensure that if any other
            // SaveChanges calls to the data source (e.g. EF Core) are called, that they are
            // transacted atomically. The isolation is set to ReadCommitted by default (i.e. read-
            // locks are released, while write-locks are maintained for the duration of the
            // transaction). Learn more on this approach for EF Core:
            // https://docs.microsoft.com/en-us/ef/core/saving/transactions#using-systemtransactions
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                response = await next();

                // By calling SaveChanges at the last point in the transaction ensures that write-
                // locks in the database are created and then released as quickly as possible. This
                // helps optimize the application to handle a higher degree of concurrency.
                await _dataSource.SaveChangesAsync(cancellationToken);

                // Commit transaction if everything succeeds, transaction will auto-rollback when
                // disposed if anything failed.
                transaction.Complete();
            }

            await _tableStorageDataSource.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}