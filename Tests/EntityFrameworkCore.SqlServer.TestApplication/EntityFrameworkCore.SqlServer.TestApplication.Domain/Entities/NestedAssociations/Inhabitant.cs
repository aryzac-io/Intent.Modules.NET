using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.NestedAssociations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Inhabitant : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string InhabitantAttribute { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        protected virtual ICollection<Branch> Branches { get; set; }
    }
}