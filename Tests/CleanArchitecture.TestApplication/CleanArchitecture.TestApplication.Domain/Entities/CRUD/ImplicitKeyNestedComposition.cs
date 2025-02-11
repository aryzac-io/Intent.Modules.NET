using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.CRUD
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class ImplicitKeyNestedComposition
    {

        public Guid Id { get; set; }

        public string Attribute { get; set; }

        public Guid ImplicitKeyAggrRootId { get; set; }
    }
}