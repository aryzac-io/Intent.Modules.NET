using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxNone.TestApplication.Eventing.Messages
{
    public record EventStartedEvent
    {
        public string Message { get; init; }
    }
}