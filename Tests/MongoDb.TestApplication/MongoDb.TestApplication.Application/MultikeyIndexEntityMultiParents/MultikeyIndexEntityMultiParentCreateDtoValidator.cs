using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntityMultiParents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MultikeyIndexEntityMultiParentCreateDtoValidator : AbstractValidator<MultikeyIndexEntityMultiParentCreateDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public MultikeyIndexEntityMultiParentCreateDtoValidator(IServiceProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IServiceProvider provider)
        {
            RuleFor(v => v.SomeField)
                .NotNull();

            RuleFor(v => v.MultikeyIndexEntityMultiChild)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetRequiredService<IValidator<MultikeyIndexEntityMultiChildDto>>()!));
        }
    }
}