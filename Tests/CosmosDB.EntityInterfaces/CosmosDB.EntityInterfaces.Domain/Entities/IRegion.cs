using System.Collections.Generic;
using CosmosDB.EntityInterfaces.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public interface IRegion : IHasDomainEvent
    {
        string Id { get; set; }

        string Name { get; set; }

        ICollection<ICountry> Countries { get; set; }
    }
}