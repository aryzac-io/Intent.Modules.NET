using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.TestNullablities.GetTestNullabilityWithNullReturn
{
    public class GetTestNullabilityWithNullReturnValidator : AbstractValidator<GetTestNullabilityWithNullReturn>
    {
        [IntentManaged(Mode.Merge)]
        public GetTestNullabilityWithNullReturnValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}