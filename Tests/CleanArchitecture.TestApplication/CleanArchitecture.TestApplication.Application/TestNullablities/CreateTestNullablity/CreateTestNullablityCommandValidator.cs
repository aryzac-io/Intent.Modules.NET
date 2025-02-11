using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.TestNullablities.CreateTestNullablity
{
    public class CreateTestNullablityCommandValidator : AbstractValidator<CreateTestNullablityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateTestNullablityCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.MyEnum)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.Str)
                .NotNull();

            RuleFor(v => v.NullableEnum)
                .IsInEnum();

            RuleFor(v => v.DefaultLiteralEnum)
                .NotNull()
                .IsInEnum();
        }
    }
}