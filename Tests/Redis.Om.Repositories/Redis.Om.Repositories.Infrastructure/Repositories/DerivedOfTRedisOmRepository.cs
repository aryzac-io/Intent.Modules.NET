using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Redis.OM;
using Redis.Om.Repositories.Domain.Entities;
using Redis.Om.Repositories.Domain.Repositories;
using Redis.Om.Repositories.Domain.Repositories.Documents;
using Redis.Om.Repositories.Infrastructure.Persistence;
using Redis.Om.Repositories.Infrastructure.Persistence.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmRepository", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Repositories
{
    internal class DerivedOfTRedisOmRepository : RedisOmRepositoryBase<DerivedOfT, DerivedOfTDocument, IDerivedOfTDocument>, IDerivedOfTRepository
    {
        public DerivedOfTRedisOmRepository(RedisOmUnitOfWork unitOfWork, RedisConnectionProvider connectionProvider) : base(unitOfWork, connectionProvider)
        {
        }

        public async Task<DerivedOfT?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);

        protected override string GetIdValue(DerivedOfT entity) => entity.Id;

        protected override void SetIdValue(DerivedOfT domainEntity, DerivedOfTDocument document) => ReflectionHelper.ForceSetProperty(domainEntity, "Id", document.Id);
    }
}