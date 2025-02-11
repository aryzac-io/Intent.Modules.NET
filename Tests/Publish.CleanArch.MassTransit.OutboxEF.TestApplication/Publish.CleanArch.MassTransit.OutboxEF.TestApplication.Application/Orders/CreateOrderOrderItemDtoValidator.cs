using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.Orders
{
    public class CreateOrderOrderItemDtoValidator : AbstractValidator<CreateOrderOrderItemDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrderOrderItemDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}