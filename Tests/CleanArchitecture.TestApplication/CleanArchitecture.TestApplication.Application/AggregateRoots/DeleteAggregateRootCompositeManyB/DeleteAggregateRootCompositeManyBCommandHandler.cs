using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots.DeleteAggregateRootCompositeManyB
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteAggregateRootCompositeManyBCommandHandler : IRequestHandler<DeleteAggregateRootCompositeManyBCommand>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public DeleteAggregateRootCompositeManyBCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(
            DeleteAggregateRootCompositeManyBCommand request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.AggregateRootId, cancellationToken);

            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(AggregateRoot)} of Id '{request.AggregateRootId}' could not be found");
            }

            var element = aggregateRoot.Composites.FirstOrDefault(p => p.Id == request.Id);

            if (element is null)
            {
                throw new NotFoundException($"{nameof(CompositeManyB)} of Id '{request.Id}' could not be found associated with {nameof(AggregateRoot)} of Id '{request.AggregateRootId}'");
            }
            aggregateRoot.Composites.Remove(element);
            return Unit.Value;
        }
    }
}