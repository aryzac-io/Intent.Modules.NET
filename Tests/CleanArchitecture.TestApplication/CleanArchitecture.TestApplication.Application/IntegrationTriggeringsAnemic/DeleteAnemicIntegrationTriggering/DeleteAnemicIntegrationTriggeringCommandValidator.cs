using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.IntegrationTriggeringsAnemic.DeleteAnemicIntegrationTriggering
{
    public class DeleteAnemicIntegrationTriggeringCommandValidator : AbstractValidator<DeleteAnemicIntegrationTriggeringCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteAnemicIntegrationTriggeringCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}