using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.ClassContainers.GetClassContainerById
{
    public class GetClassContainerByIdQueryValidator : AbstractValidator<GetClassContainerByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetClassContainerByIdQueryValidator()
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