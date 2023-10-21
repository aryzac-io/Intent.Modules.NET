using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class F_OptionalDependent : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string OptionalDepAttr { get; set; }

        public virtual F_OptionalAggregateNav? F_OptionalAggregateNav { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}