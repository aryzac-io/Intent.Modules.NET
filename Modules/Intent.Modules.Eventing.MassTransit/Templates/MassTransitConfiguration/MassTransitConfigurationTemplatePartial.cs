using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class MassTransitConfigurationTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Modules.Eventing.MassTransit.MassTransitConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MassTransitConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MassTransit);
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest.ToRegister(
                    "AddMassTransitConfiguration",
                    ServiceConfigurationRequest.ParameterType.Configuration)
                .ForConcern("Infrastructure")
                .HasDependency(this));

            switch (ExecutionContext.Settings.GetEventing().MessagingServiceProvider().AsEnum())
            {
                case Settings.Eventing.MessagingServiceProviderOptionsEnum.InMemory:
                    // InMemory doesn't require appsettings
                    break;
                case Settings.Eventing.MessagingServiceProviderOptionsEnum.Rabbitmq:
                    AddNugetDependency(NuGetPackages.MassTransitRabbitMq);
                    
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("RabbitMq:Host", "localhost"));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("RabbitMq:VirtualHost", "/"));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("RabbitMq:Username", "guest"));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("RabbitMq:Password", "guest"));
                    break;
                default:
                    throw new InvalidOperationException($"Messaging Service Provider is set to a setting that is not supported: {ExecutionContext.Settings.GetEventing().MessagingServiceProvider().AsEnum()}");
            }
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"MassTransitConfiguration",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private string GetMessagingProviderSpecificConfig()
        {
            var lines = new List<string>();

            switch (ExecutionContext.Settings.GetEventing().MessagingServiceProvider().AsEnum())
            {
                case Settings.Eventing.MessagingServiceProviderOptionsEnum.InMemory:
                    lines.Add("x.UsingInMemory((context, cfg) =>");
                    lines.Add("{");
                    lines.Add("    cfg.ConfigureEndpoints(context);");
                    lines.Add("});");
                    break;
                case Settings.Eventing.MessagingServiceProviderOptionsEnum.Rabbitmq:
                    lines.Add(@$"x.UsingRabbitMq((context, cfg) =>");
                    lines.Add(@$"{{");
                    lines.Add(@$"    cfg.Host(configuration[""RabbitMq:Host""], configuration[""RabbitMq:VirtualHost""], h => {{");
                    lines.Add(@$"        h.Username(configuration[""RabbitMq:Username""]);");
                    lines.Add(@$"        h.Password(configuration[""RabbitMq:Password""]);");
                    lines.Add(@$"    }});");
                    lines.Add(@$"    ");
                    lines.Add(@$"    cfg.ConfigureEndpoints(context);");
                    lines.Add(@$"}});");
                    break;
                default:
                    throw new InvalidOperationException($"Messaging Service Provider is set to a setting that is not supported: {ExecutionContext.Settings.GetEventing().MessagingServiceProvider().AsEnum()}");
            }

            const string newLine = @"
                ";
            return string.Join(newLine, lines);
        }
    }
}