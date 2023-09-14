using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntityMultiParents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TextIndexEntityMultiParentCreateDtoValidator : AbstractValidator<TextIndexEntityMultiParentCreateDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public TextIndexEntityMultiParentCreateDtoValidator(IServiceProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IServiceProvider provider)
        {
            RuleFor(v => v.SomeField)
                .NotNull();

            RuleFor(v => v.TextIndexEntityMultiChild)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetRequiredService<IValidator<TextIndexEntityMultiChildDto>>()!));
        }
    }
}