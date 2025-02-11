using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.ODataAggs.GetODataAggById
{
    public class GetODataAggByIdQueryValidator : AbstractValidator<GetODataAggByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetODataAggByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}