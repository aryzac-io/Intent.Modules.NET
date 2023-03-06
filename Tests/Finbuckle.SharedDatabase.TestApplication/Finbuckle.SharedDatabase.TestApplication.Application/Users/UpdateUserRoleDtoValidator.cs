using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "1.0")]

namespace Finbuckle.SharedDatabase.TestApplication.Application.Users
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateUserRoleDtoValidator : AbstractValidator<UpdateUserRoleDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public UpdateUserRoleDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

        }
    }
}