using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AggregateRootCompositeSingleADtoValidator : AbstractValidator<AggregateRootCompositeSingleADto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public AggregateRootCompositeSingleADtoValidator(IServiceProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IServiceProvider provider)
        {
            RuleFor(v => v.CompositeAttr)
                .NotNull();

            RuleFor(v => v.Composite)
                .SetValidator(provider.GetRequiredService<IValidator<AggregateRootCompositeSingleACompositeSingleAADto>>()!);

            RuleFor(v => v.Composites)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetRequiredService<IValidator<AggregateRootCompositeSingleACompositeManyAADto>>()!));
        }
    }
}