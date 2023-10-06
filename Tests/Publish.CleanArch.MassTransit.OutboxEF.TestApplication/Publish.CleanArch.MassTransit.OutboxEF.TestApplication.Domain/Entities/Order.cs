using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Common;

namespace Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}