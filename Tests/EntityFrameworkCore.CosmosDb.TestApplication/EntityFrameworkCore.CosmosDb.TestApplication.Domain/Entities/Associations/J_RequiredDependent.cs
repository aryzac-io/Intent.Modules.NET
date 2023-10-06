using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class J_RequiredDependent : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string RequiredDepAttr { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}