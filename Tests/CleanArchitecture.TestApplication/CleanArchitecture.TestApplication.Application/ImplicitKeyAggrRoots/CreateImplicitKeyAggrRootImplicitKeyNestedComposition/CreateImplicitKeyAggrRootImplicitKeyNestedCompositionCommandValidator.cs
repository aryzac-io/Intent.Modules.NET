using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRootImplicitKeyNestedComposition
{
    public class CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandValidator : AbstractValidator<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandValidator()
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