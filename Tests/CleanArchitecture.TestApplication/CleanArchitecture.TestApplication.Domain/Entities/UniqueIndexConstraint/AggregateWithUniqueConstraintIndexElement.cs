using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.TestApplication.Domain.Entities.UniqueIndexConstraint
{
    public class AggregateWithUniqueConstraintIndexElement : IHasDomainEvent
    {
        public AggregateWithUniqueConstraintIndexElement(string singleUniqueField,
            string compUniqueFieldA,
            string compUniqueFieldB)
        {
            SingleUniqueField = singleUniqueField;
            CompUniqueFieldA = compUniqueFieldA;
            CompUniqueFieldB = compUniqueFieldB;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Ignore)]
        public AggregateWithUniqueConstraintIndexElement()
        {
            SingleUniqueField = null!;
            CompUniqueFieldA = null!;
            CompUniqueFieldB = null!;
        }
        public Guid Id { get; set; }

        public string SingleUniqueField { get; set; }

        public string CompUniqueFieldA { get; set; }

        public string CompUniqueFieldB { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}