using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdTypeGuids
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class IdTypeGuidDtoValidator : AbstractValidator<IdTypeGuidDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public IdTypeGuidDtoValidator()
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