using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.UpdateImplicitKeyAggrRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateImplicitKeyAggrRootCommandHandler : IRequestHandler<UpdateImplicitKeyAggrRootCommand>
    {
        private readonly IImplicitKeyAggrRootRepository _implicitKeyAggrRootRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateImplicitKeyAggrRootCommandHandler(IImplicitKeyAggrRootRepository implicitKeyAggrRootRepository)
        {
            _implicitKeyAggrRootRepository = implicitKeyAggrRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateImplicitKeyAggrRootCommand request, CancellationToken cancellationToken)
        {
            var existingImplicitKeyAggrRoot = await _implicitKeyAggrRootRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingImplicitKeyAggrRoot is null)
            {
                throw new NotFoundException($"Could not find ImplicitKeyAggrRoot '{request.Id}'");
            }

            existingImplicitKeyAggrRoot.Attribute = request.Attribute;
            existingImplicitKeyAggrRoot.ImplicitKeyNestedCompositions = UpdateHelper.CreateOrUpdateCollection(existingImplicitKeyAggrRoot.ImplicitKeyNestedCompositions, request.ImplicitKeyNestedCompositions, (e, d) => e.Id == d.Id, CreateOrUpdateImplicitKeyNestedComposition);

        }

        [IntentManaged(Mode.Fully)]
        private static ImplicitKeyNestedComposition CreateOrUpdateImplicitKeyNestedComposition(
            ImplicitKeyNestedComposition? entity,
            UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionDto dto)
        {
            entity ??= new ImplicitKeyNestedComposition();
            entity.Attribute = dto.Attribute;

            return entity;
        }
    }
}