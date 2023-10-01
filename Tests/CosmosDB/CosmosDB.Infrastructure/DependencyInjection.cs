using System;
using System.Collections;
using CosmosDB.Application.Common.Interfaces;
using CosmosDB.Domain.Common.Interfaces;
using CosmosDB.Domain.Repositories;
using CosmosDB.Domain.Repositories.Folder;
using CosmosDB.Infrastructure.Persistence;
using CosmosDB.Infrastructure.Persistence.Documents;
using CosmosDB.Infrastructure.Persistence.Documents.Folder;
using CosmosDB.Infrastructure.Repositories;
using CosmosDB.Infrastructure.Repositories.Folder;
using CosmosDB.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CosmosDB.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCosmosRepository(options =>
            {
                var defaultContainerId = configuration.GetValue<string>("RepositoryOptions:ContainerId");

                if (string.IsNullOrWhiteSpace(defaultContainerId))
                {
                    throw new Exception("\"RepositoryOptions:ContainerId\" configuration not specified");
                }

                options.ContainerPerItemType = true;

                options.ContainerBuilder
                    .Configure<BaseTypeDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<ClassContainerDocument>(c => c
                        .WithContainer("Class")
                        .WithPartitionKey("/classPartitionKey"))
                    .Configure<ClientDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<DerivedOfTDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<DerivedTypeDocument>(c => c
                        .WithContainer(defaultContainerId))
                    // Cosmos Repository does not support open generics at this time, but it's good
                    // to test everything else is still correct:
                    // .Configure(typeof(EntityOfTDocument<>), c => c
                    //     .WithContainer(defaultContainerId))
                    .Configure<FolderContainerDocument>(c => c
                        .WithContainer("Folder")
                        .WithPartitionKey("/folderPartitionKey"))
                    .Configure<IdTestingDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<InvoiceDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<PackageContainerDocument>(c => c
                        .WithContainer("PackageContainer")
                        .WithPartitionKey("/packagePartitionKey"))
                    .Configure<RegionDocument>(c => c
                        .WithContainer(defaultContainerId))
                    .Configure<WithoutPartitionKeyDocument>(c => c
                        .WithContainer("WithoutPartitionKey"));
            });
            services.AddScoped<IBaseTypeRepository, BaseTypeCosmosDBRepository>();
            services.AddScoped<IClassContainerRepository, ClassContainerCosmosDBRepository>();
            services.AddScoped<IClientRepository, ClientCosmosDBRepository>();
            services.AddScoped<IDerivedOfTRepository, DerivedOfTCosmosDBRepository>();
            services.AddScoped<IDerivedTypeRepository, DerivedTypeCosmosDBRepository>();
            services.AddScoped(typeof(IEntityOfTRepository<>), typeof(EntityOfTCosmosDBRepository<>));
            services.AddScoped<IIdTestingRepository, IdTestingCosmosDBRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceCosmosDBRepository>();
            services.AddScoped<IPackageContainerRepository, PackageContainerCosmosDBRepository>();
            services.AddScoped<IRegionRepository, RegionCosmosDBRepository>();
            services.AddScoped<IWithoutPartitionKeyRepository, WithoutPartitionKeyCosmosDBRepository>();
            services.AddScoped<IFolderContainerRepository, FolderContainerCosmosDBRepository>();
            services.AddScoped<CosmosDBUnitOfWork>();
            services.AddScoped<ICosmosDBUnitOfWork>(provider => provider.GetRequiredService<CosmosDBUnitOfWork>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}