using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.Versioned.TestCommandV2
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestCommandV2Handler : IRequestHandler<TestCommandV2>
    {
        public const string ExpectedInput = "456";

        [IntentManaged(Mode.Merge)]
        public TestCommandV2Handler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(TestCommandV2 request, CancellationToken cancellationToken)
        {
            Assert.Equal(ExpectedInput, request.Value);

        }
    }
}