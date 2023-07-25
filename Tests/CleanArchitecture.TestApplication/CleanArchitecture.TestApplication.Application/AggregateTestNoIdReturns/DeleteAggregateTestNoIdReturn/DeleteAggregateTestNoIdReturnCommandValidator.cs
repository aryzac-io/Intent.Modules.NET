using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateTestNoIdReturns.DeleteAggregateTestNoIdReturn
{
    public class DeleteAggregateTestNoIdReturnCommandValidator : AbstractValidator<DeleteAggregateTestNoIdReturnCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public DeleteAggregateTestNoIdReturnCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}