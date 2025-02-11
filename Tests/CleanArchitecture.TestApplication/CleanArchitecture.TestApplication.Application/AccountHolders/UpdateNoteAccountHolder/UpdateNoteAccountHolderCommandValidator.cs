using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.AccountHolders.UpdateNoteAccountHolder
{
    public class UpdateNoteAccountHolderCommandValidator : AbstractValidator<UpdateNoteAccountHolderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateNoteAccountHolderCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Note)
                .NotNull();
        }
    }
}