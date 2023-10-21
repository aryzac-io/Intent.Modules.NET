using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Indexes
{
    public class SortDirectionStereotype : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string FieldA { get; set; }

        public string FieldB { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}