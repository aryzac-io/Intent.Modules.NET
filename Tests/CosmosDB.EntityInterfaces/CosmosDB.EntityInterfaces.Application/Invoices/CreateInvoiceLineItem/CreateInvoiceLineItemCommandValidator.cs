using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Application.Invoices.CreateInvoiceLineItem
{
    public class CreateInvoiceLineItemCommandValidator : AbstractValidator<CreateInvoiceLineItemCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateInvoiceLineItemCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.InvoiceId)
                .NotNull();

            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}