using System;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.Messages.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Subscribe.MassTransit.OutboxMemory.Application.Common.Eventing;
using Subscribe.MassTransit.OutboxMemory.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace Subscribe.MassTransit.OutboxMemory.Infrastructure.Configuration
{
    public static class MassTransitConfiguration
    {
        public static void AddMassTransitConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<MassTransitEventBus>();
            services.AddScoped<IEventBus>(provider => provider.GetRequiredService<MassTransitEventBus>());

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumers();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseMessageRetry(r => r.Interval(
                        configuration.GetValue<int?>("MassTransit:RetryInterval:RetryCount") ?? 10,
                        configuration.GetValue<TimeSpan?>("MassTransit:RetryInterval:Interval") ?? TimeSpan.FromSeconds(5)));

                    cfg.Host(configuration["RabbitMq:Host"], configuration["RabbitMq:VirtualHost"], host =>
                    {
                        host.Username(configuration["RabbitMq:Username"]);
                        host.Password(configuration["RabbitMq:Password"]);
                    });
                    cfg.ConfigureEndpoints(context);
                    cfg.UseInMemoryOutbox();
                    cfg.AddMessageTopologyConfiguration();
                });
                x.AddInMemoryInboxOutbox();
            });
        }

        private static void AddMessageTopologyConfiguration(this IRabbitMqBusFactoryConfigurator cfg)
        {
            cfg.Message<BasketCreatedEvent>(x => x.SetEntityName("basket-created-topic-rename"));
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<BasketCreatedEvent>, BasketCreatedEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<BasketCreatedEvent>, BasketCreatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<BasketUpdatedEvent>, BasketUpdatedEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<BasketUpdatedEvent>, BasketUpdatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<BasketDeletedEvent>, BasketDeletedEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<BasketDeletedEvent>, BasketDeletedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<BasketItemCreatedEvent>, BasketItemCreatedEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<BasketItemCreatedEvent>, BasketItemCreatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<BasketItemUpdatedEvent>, BasketItemUpdatedEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<BasketItemUpdatedEvent>, BasketItemUpdatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<BasketItemDeletedEvent>, BasketItemDeletedEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<BasketItemDeletedEvent>, BasketItemDeletedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<RoleCreatedEvent>, RoleCreatedEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<RoleCreatedEvent>, RoleCreatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<RoleUpdatedEvent>, RoleUpdatedEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<RoleUpdatedEvent>, RoleUpdatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<RoleDeletedEvent>, RoleDeletedEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<RoleDeletedEvent>, RoleDeletedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<DelayedNotificationEvent>, DelayedNotificationEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<DelayedNotificationEvent>, DelayedNotificationEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
        }
    }
}