using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Interfaces;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.Async;
using CleanArchitecture.TestApplication.Domain.Entities.CompositeKeys;
using CleanArchitecture.TestApplication.Domain.Entities.ConventionBasedEventPublishing;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Entities.DDD;
using CleanArchitecture.TestApplication.Domain.Entities.DefaultDiagram;
using CleanArchitecture.TestApplication.Domain.Entities.Enums;
using CleanArchitecture.TestApplication.Domain.Entities.General;
using CleanArchitecture.TestApplication.Domain.Entities.Inheritance;
using CleanArchitecture.TestApplication.Domain.Entities.Nullability;
using CleanArchitecture.TestApplication.Domain.Entities.ODataQuery;
using CleanArchitecture.TestApplication.Domain.Entities.OperationAndConstructorMapping;
using CleanArchitecture.TestApplication.Domain.Entities.Operations;
using CleanArchitecture.TestApplication.Domain.Entities.Pagination;
using CleanArchitecture.TestApplication.Domain.Entities.UniqueIndexConstraint;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.Async;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.CompositeKeys;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.ConventionBasedEventPublishing;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.CRUD;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.DDD;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.DefaultDiagram;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.Enums;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.General;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.Inheritance;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.Nullability;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.ODataQuery;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.OperationAndConstructorMapping;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.Operations;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.Pagination;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IDomainEventService _domainEventService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }

        public DbSet<Person> People { get; set; }

        public DbSet<Camera> Cameras { get; set; }

        public DbSet<WithCompositeKey> WithCompositeKeys { get; set; }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountHolder> AccountHolders { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<AggregateRoot> AggregateRoots { get; set; }
        public DbSet<AggregateRootLong> AggregateRootLongs { get; set; }
        public DbSet<AggregateSingleC> AggregateSingleCs { get; set; }
        public DbSet<AggregateTestNoIdReturn> AggregateTestNoIdReturns { get; set; }
        public DbSet<AsyncOperationsClass> AsyncOperationsClasses { get; set; }
        public DbSet<IntegrationTriggering> IntegrationTriggerings { get; set; }
        public DbSet<ClassWithDefault> ClassWithDefaults { get; set; }
        public DbSet<ClassWithEnums> ClassWithEnums { get; set; }
        public DbSet<CustomMapping> CustomMappings { get; set; }
        public DbSet<BaseClass> BaseClasses { get; set; }
        public DbSet<ConcreteClass> ConcreteClasses { get; set; }
        public DbSet<CompositeManyB> CompositeManyBs { get; set; }
        public DbSet<CompositeSingleA> CompositeSingleAs { get; set; }
        public DbSet<CompositeSingleAA> CompositeSingleAAs { get; set; }
        public DbSet<CompositeSingleBB> CompositeSingleBBs { get; set; }
        public DbSet<DataContractClass> DataContractClasses { get; set; }
        public DbSet<ImplicitKeyAggrRoot> ImplicitKeyAggrRoots { get; set; }
        public DbSet<NullabilityPeer> NullabilityPeers { get; set; }
        public DbSet<TestNullablity> TestNullablities { get; set; }
        public DbSet<ODataAgg> ODataAggs { get; set; }
        public DbSet<OpAndCtorMapping2> OpAndCtorMapping2s { get; set; }
        public DbSet<OpAndCtorMapping3> OpAndCtorMapping3s { get; set; }
        public DbSet<OperationsClass> OperationsClasses { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<AggregateWithUniqueConstraintIndexElement> AggregateWithUniqueConstraintIndexElements { get; set; }
        public DbSet<AggregateWithUniqueConstraintIndexStereotype> AggregateWithUniqueConstraintIndexStereotypes { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            await DispatchEventsAsync(cancellationToken);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            DispatchEventsAsync().GetAwaiter().GetResult();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new AsyncOperationsClassConfiguration());
            modelBuilder.ApplyConfiguration(new WithCompositeKeyConfiguration());
            modelBuilder.ApplyConfiguration(new IntegrationTriggeringConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRootConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRootLongConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateSingleCConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateTestNoIdReturnConfiguration());
            modelBuilder.ApplyConfiguration(new CompositeManyBConfiguration());
            modelBuilder.ApplyConfiguration(new CompositeSingleAConfiguration());
            modelBuilder.ApplyConfiguration(new CompositeSingleAAConfiguration());
            modelBuilder.ApplyConfiguration(new CompositeSingleBBConfiguration());
            modelBuilder.ApplyConfiguration(new ImplicitKeyAggrRootConfiguration());
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new AccountHolderConfiguration());
            modelBuilder.ApplyConfiguration(new CameraConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new DataContractClassConfiguration());
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new ClassWithDefaultConfiguration());
            modelBuilder.ApplyConfiguration(new ClassWithEnumsConfiguration());
            modelBuilder.ApplyConfiguration(new CustomMappingConfiguration());
            modelBuilder.ApplyConfiguration(new BaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new ConcreteClassConfiguration());
            modelBuilder.ApplyConfiguration(new NullabilityPeerConfiguration());
            modelBuilder.ApplyConfiguration(new TestNullablityConfiguration());
            modelBuilder.ApplyConfiguration(new ODataAggConfiguration());
            modelBuilder.ApplyConfiguration(new OpAndCtorMapping2Configuration());
            modelBuilder.ApplyConfiguration(new OpAndCtorMapping3Configuration());
            modelBuilder.ApplyConfiguration(new OperationsClassConfiguration());
            modelBuilder.ApplyConfiguration(new LogEntryConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateWithUniqueConstraintIndexElementConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateWithUniqueConstraintIndexStereotypeConfiguration());
        }

        [IntentManaged(Mode.Ignore)]
        private void ConfigureModel(ModelBuilder modelBuilder)
        {
            // Seed data
            // https://rehansaeed.com/migrating-to-entity-framework-core-seed-data/
            /* Eg.
            
            modelBuilder.Entity<Car>().HasData(
            new Car() { CarId = 1, Make = "Ferrari", Model = "F40" },
            new Car() { CarId = 2, Make = "Ferrari", Model = "F50" },
            new Car() { CarId = 3, Make = "Labourghini", Model = "Countach" });
            */
        }

        private async Task DispatchEventsAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var domainEventEntity = ChangeTracker
                    .Entries<IHasDomainEvent>()
                    .Select(x => x.Entity.DomainEvents)
                    .SelectMany(x => x)
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