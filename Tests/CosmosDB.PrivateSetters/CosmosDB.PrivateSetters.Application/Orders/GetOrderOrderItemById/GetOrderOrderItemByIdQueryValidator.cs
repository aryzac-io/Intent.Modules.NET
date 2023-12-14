using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Orders.GetOrderOrderItemById
{
    public class GetOrderOrderItemByIdQueryValidator : AbstractValidator<GetOrderOrderItemByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrderOrderItemByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.OrderId)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.WarehouseId)
                .NotNull();
        }
    }
}