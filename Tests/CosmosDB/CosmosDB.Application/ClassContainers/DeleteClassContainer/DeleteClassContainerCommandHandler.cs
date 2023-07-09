using System;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CosmosDB.Application.ClassContainers.DeleteClassContainer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteClassContainerCommandHandler : IRequestHandler<DeleteClassContainerCommand>
    {
        private readonly IClassContainerRepository _classContainerRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteClassContainerCommandHandler(IClassContainerRepository classContainerRepository)
        {
            _classContainerRepository = classContainerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(DeleteClassContainerCommand request, CancellationToken cancellationToken)
        {
            var existingClassContainer = await _classContainerRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingClassContainer is null)
            {
                throw new NotFoundException($"Could not find ClassContainer '{request.Id}' ");
            }
            _classContainerRepository.Remove(existingClassContainer);
            return Unit.Value;
        }
    }
}