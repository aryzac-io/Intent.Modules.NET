using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootCommandHandler : IRequestHandler<CreateAggregateRootCommand>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public CreateAggregateRootCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(CreateAggregateRootCommand request, CancellationToken cancellationToken)
        {
            var newAggregateRoot = new AggregateRoot
            {
                AggregateAttr = request.AggregateAttr,
                Composites = request.Composites.Select(CreateCompositeManyB).ToList(),
                Composite = request.Composite != null ? CreateCompositeSingleA(request.Composite) : null,
#warning Field not a composite association: Aggregate
            };

            _aggregateRootRepository.Add(newAggregateRoot);
            return Unit.Value;
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeManyB CreateCompositeManyB(CreateAggregateRootCompositeManyBDto dto)
        {
            return new CompositeManyB
            {
                CompositeAttr = dto.CompositeAttr,
                SomeDate = dto.SomeDate,
                Composite = dto.Composite != null ? CreateCompositeSingleBB(dto.Composite) : null,
                Composites = dto.Composites.Select(CreateCompositeManyBB).ToList(),
            };
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeSingleBB CreateCompositeSingleBB(CreateAggregateRootCompositeManyBCompositeSingleBBDto dto)
        {
            return new CompositeSingleBB
            {
                CompositeAttr = dto.CompositeAttr,
            };
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeManyBB CreateCompositeManyBB(CreateAggregateRootCompositeManyBCompositeManyBBDto dto)
        {
            return new CompositeManyBB
            {
                CompositeAttr = dto.CompositeAttr,
            };
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeSingleA CreateCompositeSingleA(CreateAggregateRootCompositeSingleADto dto)
        {
            return new CompositeSingleA
            {
                CompositeAttr = dto.CompositeAttr,
                Composite = dto.Composite != null ? CreateCompositeSingleAA(dto.Composite) : null,
                Composites = dto.Composites.Select(CreateCompositeManyAA).ToList(),
            };
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeSingleAA CreateCompositeSingleAA(CreateAggregateRootCompositeSingleACompositeSingleAADto dto)
        {
            return new CompositeSingleAA
            {
                CompositeAttr = dto.CompositeAttr,
            };
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeManyAA CreateCompositeManyAA(CreateAggregateRootCompositeSingleACompositeManyAADto dto)
        {
            return new CompositeManyAA
            {
                CompositeAttr = dto.CompositeAttr,
            };
        }
    }
}