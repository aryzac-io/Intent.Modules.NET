using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Orders.UpdateOrderOrderItem
{
    public class UpdateOrderOrderItemCommandValidator : AbstractValidator<UpdateOrderOrderItemCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateOrderOrderItemCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}