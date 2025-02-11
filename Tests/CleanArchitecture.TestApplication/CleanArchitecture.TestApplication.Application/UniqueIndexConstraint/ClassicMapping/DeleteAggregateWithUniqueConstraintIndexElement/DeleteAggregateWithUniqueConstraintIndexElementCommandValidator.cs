using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.UniqueIndexConstraint.ClassicMapping.DeleteAggregateWithUniqueConstraintIndexElement
{
    public class DeleteAggregateWithUniqueConstraintIndexElementCommandValidator : AbstractValidator<DeleteAggregateWithUniqueConstraintIndexElementCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteAggregateWithUniqueConstraintIndexElementCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}