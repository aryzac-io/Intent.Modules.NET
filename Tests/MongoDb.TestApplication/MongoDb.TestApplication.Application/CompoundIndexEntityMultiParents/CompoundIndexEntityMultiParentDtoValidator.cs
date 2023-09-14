using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntityMultiParents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CompoundIndexEntityMultiParentDtoValidator : AbstractValidator<CompoundIndexEntityMultiParentDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public CompoundIndexEntityMultiParentDtoValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.SomeField)
                .NotNull();
        }
    }
}