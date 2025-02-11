using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRootImplicitKeyNestedComposition
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler : IRequestHandler<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand, Guid>
    {
        private readonly IImplicitKeyAggrRootRepository _implicitKeyAggrRootRepository;

        [IntentManaged(Mode.Merge)]
        public CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(IImplicitKeyAggrRootRepository implicitKeyAggrRootRepository)
        {
            _implicitKeyAggrRootRepository = implicitKeyAggrRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(
            CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _implicitKeyAggrRootRepository.FindByIdAsync(request.ImplicitKeyAggrRootId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(ImplicitKeyAggrRoot)} of Id '{request.ImplicitKeyAggrRootId}' could not be found");
            }

            var newImplicitKeyNestedComposition = new ImplicitKeyNestedComposition
            {
#warning No matching field found for ImplicitKeyAggrRootId
                Attribute = request.Attribute,
            };

            aggregateRoot.ImplicitKeyNestedCompositions.Add(newImplicitKeyNestedComposition);
            await _implicitKeyAggrRootRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newImplicitKeyNestedComposition.Id;
        }
    }
}