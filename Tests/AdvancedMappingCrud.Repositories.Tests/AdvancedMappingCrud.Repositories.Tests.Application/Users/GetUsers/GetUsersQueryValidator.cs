using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users.GetUsers
{
    public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetUsersQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}