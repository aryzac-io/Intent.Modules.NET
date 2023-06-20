using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots.UpdateAggregateRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateRootCommandHandler : IRequestHandler<UpdateAggregateRootCommand>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public UpdateAggregateRootCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(UpdateAggregateRootCommand request, CancellationToken cancellationToken)
        {
            var existingAggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingAggregateRoot is null)
            {
                throw new NotFoundException($"Could not find AggregateRoot {request.Id}");
            }
            existingAggregateRoot.AggregateAttr = request.AggregateAttr;
            existingAggregateRoot.Composites = UpdateHelper.CreateOrUpdateCollection(existingAggregateRoot.Composites, request.Composites, (e, d) => e.Id == d.Id, CreateOrUpdateCompositeManyB);
            existingAggregateRoot.Composite = CreateOrUpdateCompositeSingleA(existingAggregateRoot.Composite, request.Composite);
#warning Field not a composite association: Aggregate
            return Unit.Value;
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeManyB CreateOrUpdateCompositeManyB(
            CompositeManyB entity,
            UpdateAggregateRootCompositeManyBDto dto)
        {

            entity ??= new CompositeManyB();
            entity.CompositeAttr = dto.CompositeAttr;
            entity.SomeDate = dto.SomeDate;
            entity.AggregateRootId = dto.AggregateRootId;
            entity.Composites = UpdateHelper.CreateOrUpdateCollection(entity.Composites, dto.Composites, (e, d) => e.Id == d.Id, CreateOrUpdateCompositeManyBB);
            entity.Composite = CreateOrUpdateCompositeSingleBB(entity.Composite, dto.Composite);

            return entity;
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeManyBB CreateOrUpdateCompositeManyBB(
            CompositeManyBB entity,
            UpdateAggregateRootCompositeManyBCompositeManyBBDto dto)
        {

            entity ??= new CompositeManyBB();
            entity.CompositeAttr = dto.CompositeAttr;
            entity.CompositeManyBId = dto.CompositeManyBId;

            return entity;
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeSingleBB? CreateOrUpdateCompositeSingleBB(
            CompositeSingleBB? entity,
            UpdateAggregateRootCompositeManyBCompositeSingleBBDto? dto)
        {
            if (dto == null)
            {
                return null;
            }

            entity ??= new CompositeSingleBB();
            entity.CompositeAttr = dto.CompositeAttr;

            return entity;
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeSingleA? CreateOrUpdateCompositeSingleA(
            CompositeSingleA? entity,
            UpdateAggregateRootCompositeSingleADto? dto)
        {
            if (dto == null)
            {
                return null;
            }

            entity ??= new CompositeSingleA();
            entity.CompositeAttr = dto.CompositeAttr;
            entity.Composite = CreateOrUpdateCompositeSingleAA(entity.Composite, dto.Composite);
            entity.Composites = UpdateHelper.CreateOrUpdateCollection(entity.Composites, dto.Composites, (e, d) => e.Id == d.Id, CreateOrUpdateCompositeManyAA);

            return entity;
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeSingleAA? CreateOrUpdateCompositeSingleAA(
            CompositeSingleAA? entity,
            UpdateAggregateRootCompositeSingleACompositeSingleAADto? dto)
        {
            if (dto == null)
            {
                return null;
            }

            entity ??= new CompositeSingleAA();
            entity.CompositeAttr = dto.CompositeAttr;

            return entity;
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeManyAA CreateOrUpdateCompositeManyAA(
            CompositeManyAA entity,
            UpdateAggregateRootCompositeSingleACompositeManyAADto dto)
        {

            entity ??= new CompositeManyAA();
            entity.CompositeAttr = dto.CompositeAttr;
            entity.CompositeSingleAId = dto.CompositeSingleAId;

            return entity;
        }
    }
}