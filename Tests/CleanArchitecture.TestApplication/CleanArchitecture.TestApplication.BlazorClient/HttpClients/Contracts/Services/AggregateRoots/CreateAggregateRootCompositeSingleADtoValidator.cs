using CleanArchitecture.TestApplication.BlazorClient.HttpClients.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootCompositeSingleADtoValidator : AbstractValidator<CreateAggregateRootCompositeSingleADto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAggregateRootCompositeSingleADtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.CompositeAttr)
                .NotNull();

            RuleFor(v => v.Composite)
                .SetValidator(provider.GetValidator<CreateAggregateRootCompositeSingleACompositeSingleAADto>()!);

            RuleFor(v => v.Composites)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateAggregateRootCompositeSingleACompositeManyAADto>()!));
        }
    }
}