using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "1.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CompoundIndexEntityCreateDtoValidator : AbstractValidator<CompoundIndexEntityCreateDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public CompoundIndexEntityCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.SomeField)
                .NotNull();

            RuleFor(v => v.CompoundOne)
                .NotNull();

            RuleFor(v => v.CompoundTwo)
                .NotNull();
        }
    }
}