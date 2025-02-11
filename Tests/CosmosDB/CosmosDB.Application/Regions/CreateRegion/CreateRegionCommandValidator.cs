using CosmosDB.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.Application.Regions.CreateRegion
{
    public class CreateRegionCommandValidator : AbstractValidator<CreateRegionCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateRegionCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Countries)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateRegionCountryDto>()!));
        }
    }
}