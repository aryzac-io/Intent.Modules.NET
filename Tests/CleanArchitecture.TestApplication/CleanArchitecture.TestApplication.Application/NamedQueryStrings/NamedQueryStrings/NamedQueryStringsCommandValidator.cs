using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.NamedQueryStrings.NamedQueryStrings
{
    public class NamedQueryStringsCommandValidator : AbstractValidator<NamedQueryStringsCommand>
    {
        [IntentManaged(Mode.Merge)]
        public NamedQueryStringsCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Par1)
                .NotNull();
        }
    }
}