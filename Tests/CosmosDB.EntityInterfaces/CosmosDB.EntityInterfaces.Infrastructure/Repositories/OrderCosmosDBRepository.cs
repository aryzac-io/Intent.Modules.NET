using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EntityInterfaces.Application.Common.Interfaces;
using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using CosmosDB.EntityInterfaces.Infrastructure.Persistence;
using CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Repositories
{
    internal class OrderCosmosDBRepository : CosmosDBRepositoryBase<IOrder, Order, OrderDocument, IOrderDocument>, IOrderRepository
    {
        public OrderCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<OrderDocument> cosmosRepository,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", currentUserService)
        {
        }

        public async Task<IOrder?> FindByIdAsync(
            (string Id, string WarehouseId) id,
            CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id.Id, partitionKey: id.WarehouseId, cancellationToken: cancellationToken);
    }
}