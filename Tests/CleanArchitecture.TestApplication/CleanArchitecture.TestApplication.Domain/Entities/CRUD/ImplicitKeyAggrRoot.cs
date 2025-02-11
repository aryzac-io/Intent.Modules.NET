using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.CRUD
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class ImplicitKeyAggrRoot : IHasDomainEvent
    {

        public Guid Id { get; set; }

        public string Attribute { get; set; }

        public virtual ICollection<ImplicitKeyNestedComposition> ImplicitKeyNestedCompositions { get; set; } = new List<ImplicitKeyNestedComposition>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}