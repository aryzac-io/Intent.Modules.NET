using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.Application.Invoices.GetInvoiceById
{
    public class GetInvoiceByIdQueryValidator : AbstractValidator<GetInvoiceByIdQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetInvoiceByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}