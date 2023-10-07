using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.UniqueIndexConstraint.UpdateAggregateWithUniqueConstraintIndexElement
{
    public class UpdateAggregateWithUniqueConstraintIndexElementCommand : IRequest, ICommand
    {
        public UpdateAggregateWithUniqueConstraintIndexElementCommand(Guid id,
            string singleUniqueField,
            string compUniqueFieldA,
            string compUniqueFieldB)
        {
            Id = id;
            SingleUniqueField = singleUniqueField;
            CompUniqueFieldA = compUniqueFieldA;
            CompUniqueFieldB = compUniqueFieldB;
        }

        public Guid Id { get; private set; }
        public string SingleUniqueField { get; set; }
        public string CompUniqueFieldA { get; set; }
        public string CompUniqueFieldB { get; set; }

        public void SetId(Guid id)
        {
            Id = id;
        }
    }
}