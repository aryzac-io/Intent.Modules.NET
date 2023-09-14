using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.UpdateCustomerRich
{
    public class UpdateCustomerRichCommandValidator : AbstractValidator<UpdateCustomerRichCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public UpdateCustomerRichCommandValidator(IServiceProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IServiceProvider provider)
        {
            RuleFor(v => v.Address)
                .NotNull()
                .SetValidator(provider.GetRequiredService<IValidator<UpdateCustomerRichAddressDto>>()!);
        }
    }
}