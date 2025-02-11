using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Clients.GetClientsPaged
{
    public class GetClientsPagedQueryValidator : AbstractValidator<GetClientsPagedQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetClientsPagedQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}