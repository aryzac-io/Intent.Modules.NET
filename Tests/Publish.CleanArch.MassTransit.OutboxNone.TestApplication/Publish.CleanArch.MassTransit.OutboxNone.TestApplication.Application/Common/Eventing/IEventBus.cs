using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.EventBusInterface", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Eventing
{
    public interface IEventBus
    {
        void Publish<T>(T message) where T : class;
        Task FlushAllAsync(CancellationToken cancellationToken = default);
        void SchedulePublish<T>(T message, DateTime scheduled)
            where T : class;
        void SchedulePublish<T>(T message, TimeSpan delay)
            where T : class;
    }
}