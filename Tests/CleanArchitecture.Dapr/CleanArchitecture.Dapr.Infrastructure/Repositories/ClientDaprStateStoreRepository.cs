using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Domain.Entities;
using CleanArchitecture.Dapr.Domain.Repositories;
using CleanArchitecture.Dapr.Infrastructure.Persistence;
using Dapr.Client;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreRepository", Version = "1.0")]

namespace CleanArchitecture.Dapr.Infrastructure.Repositories
{
    public class ClientDaprStateStoreRepository : DaprStateStoreRepositoryBase<Client>, IClientRepository
    {
        public ClientDaprStateStoreRepository(DaprClient daprClient, DaprStateStoreUnitOfWork unitOfWork) : base(daprClient: daprClient, unitOfWork: unitOfWork, enableTransactions: false, storeName: "statestore")
        {
        }

        public void Add(Client entity)
        {
            Upsert(entity.Id, entity);
        }

        public void Update(Client entity)
        {
            Upsert(entity.Id, entity);
        }

        public void Remove(Client entity)
        {
            Remove(entity.Id, entity);
        }

        public Task<Client?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return FindByKeyAsync(id, cancellationToken);
        }

        public Task<List<Client>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default)
        {
            return FindByKeysAsync(ids, cancellationToken);
        }
    }
}