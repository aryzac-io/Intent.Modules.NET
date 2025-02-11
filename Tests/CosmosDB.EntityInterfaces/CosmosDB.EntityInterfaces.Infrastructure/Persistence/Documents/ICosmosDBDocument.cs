using System;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentOfTInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal interface ICosmosDBDocument<in TDomain, TDomainState, out TDocument> : ICosmosDBDocument
        where TDomain : class
        where TDomainState : class, TDomain
        where TDocument : ICosmosDBDocument<TDomain, TDomainState, TDocument>
    {
        TDocument PopulateFromEntity(TDomain entity);
        TDomainState ToEntity(TDomainState? entity = null);
    }

    internal interface ICosmosDBDocument : IItem
    {
        string IItem.PartitionKey => PartitionKey!;
        new string? PartitionKey
        {
            get => Id;
            set => Id = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}