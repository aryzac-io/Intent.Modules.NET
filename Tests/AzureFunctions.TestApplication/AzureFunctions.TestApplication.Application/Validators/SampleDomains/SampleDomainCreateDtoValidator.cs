using System;
using AzureFunctions.TestApplication.Application.SampleDomains;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.Validators.SampleDomains
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SampleDomainCreateDtoValidator : AbstractValidator<SampleDomainCreateDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public SampleDomainCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Attribute)
                .NotNull();
        }
    }
}