using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.ClassContainers.DeleteClassContainer
{
    public class DeleteClassContainerCommandValidator : AbstractValidator<DeleteClassContainerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteClassContainerCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.ClassPartitionKey)
                .NotNull();
        }
    }
}