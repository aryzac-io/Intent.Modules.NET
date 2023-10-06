using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.TestApplication.Domain.Entities.CRUD
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class AggregateSingleC : IHasDomainEvent
    {
        public AggregateSingleC()
        {
            AggregationAttr = null!;
        }

        public Guid Id { get; set; }

        public string AggregationAttr { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}