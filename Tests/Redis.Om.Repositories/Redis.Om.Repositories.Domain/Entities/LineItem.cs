using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Redis.Om.Repositories.Domain.Entities
{
    public class LineItem
    {
        private string? _id;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public ICollection<string> Tags { get; set; } = new List<string>();

        public Product Product { get; set; }
    }
}