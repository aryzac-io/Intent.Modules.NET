using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Repositories.Nullability;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.TestNullablities.UpdateTestNullablity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateTestNullablityCommandHandler : IRequestHandler<UpdateTestNullablityCommand>
    {
        private readonly ITestNullablityRepository _testNullablityRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateTestNullablityCommandHandler(ITestNullablityRepository testNullablityRepository)
        {
            _testNullablityRepository = testNullablityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(UpdateTestNullablityCommand request, CancellationToken cancellationToken)
        {
            var existingTestNullablity = await _testNullablityRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingTestNullablity is null)
            {
                throw new NotFoundException($"Could not find TestNullablity {request.Id}");
            }
            existingTestNullablity.SampleEnum = request.SampleEnum;
            existingTestNullablity.Str = request.Str;
            existingTestNullablity.Date = request.Date;
            existingTestNullablity.DateTime = request.DateTime;
            existingTestNullablity.NullableGuid = request.NullableGuid;
            existingTestNullablity.NullableEnum = request.NullableEnum;
            existingTestNullablity.NullabilityPeerId = request.NullabilityPeerId;
            existingTestNullablity.DefaultLiteralEnum = request.DefaultLiteralEnum;
            return Unit.Value;
        }
    }
}