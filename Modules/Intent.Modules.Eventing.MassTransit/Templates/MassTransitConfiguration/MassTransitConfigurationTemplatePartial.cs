using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Intent.Engine;
using Intent.Eventing.MassTransit.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class MassTransitConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Eventing.MassTransit.MassTransitConfiguration";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public MassTransitConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NuGetPackages.MassTransit);

        var appModels = ExecutionContext.MetadataManager
            .Eventing(ExecutionContext.GetApplicationConfig().Id).GetApplicationModels();

        MessagesWithSettings = appModels
            .SelectMany(x => x.SubscribedMessages())
            .Select(x => x.TypeReference.Element.AsMessageModel())
            .Union(appModels.SelectMany(x => x.PublishedMessages())
                .Select(x => x.TypeReference.Element.AsMessageModel()))
            .Where(p => p.HasMessageTopologySettings() && !string.IsNullOrWhiteSpace(p.GetMessageTopologySettings().EntityName()))
            .ToList();

        MessageHandlerModels = appModels
            .SelectMany(x => x.SubscribedMessages())
            .ToList();

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("System")
            .AddUsing("System.Reflection")
            .AddUsing("MassTransit")
            .AddUsing("MassTransit.Configuration")
            .AddUsing("Microsoft.Extensions.Configuration")
            .AddUsing("Microsoft.Extensions.DependencyInjection")
            .AddClass($"MassTransitConfiguration", @class =>
            {
                @class.Static();
                @class.AddMethod("void", "AddMassTransitConfiguration", method =>
                {
                    method.Static();
                    method.AddParameter("IServiceCollection", "services", parm => parm.WithThisModifier());
                    method.AddParameter("IConfiguration", "configuration");
                    method.AddStatements(GetContainerRegistrationStatements());
                    method.AddInvocationStatement("services.AddMassTransit", stmt => stmt
                        .AddArgument(GetConfigurationForAddMassTransit("configuration"))
                        .AddMetadata("configure-masstransit", true)
                        .SeparatedFromPrevious());
                });
                AddMessageTopologyConfiguration(@class);
                @class.AddMethod("void", "AddConsumers", method =>
                {
                    method.Private().Static();
                    method.AddParameter("IRegistrationConfigurator", "cfg", parm => parm.WithThisModifier());
                    method.AddStatements(GetConsumerStatements("cfg"));
                });
                AddNonDefaultEndpointConfigurationMethods(@class);
            });
    }

    private IReadOnlyCollection<MessageModel> MessagesWithSettings { get; }
    private IReadOnlyCollection<MessageSubscribeAssocationTargetEndModel> MessageHandlerModels { get; }

    private IEnumerable<CSharpStatement> GetContainerRegistrationStatements()
    {
        var statements = new List<CSharpStatement>();

        statements.Add($@"services.AddScoped<{this.GetMassTransitEventBusName()}>();");
        statements.Add($@"services.AddScoped<{this.GetEventBusInterfaceName()}>(provider => provider.GetRequiredService<{this.GetMassTransitEventBusName()}>());");

        return statements;
    }

    private CSharpLambdaBlock GetConfigurationForAddMassTransit(string configurationVarName)
    {
        var block = new CSharpLambdaBlock("x")
            .AddStatement($"x.SetKebabCaseEndpointNameFormatter();")
            .AddStatement($"x.AddConsumers();");

        if (ExecutionContext.Settings.GetEventingSettings().EnableScheduledPublishing())
        {
            block.AddStatement($"x.AddDelayedMessageScheduler();");
        }

        switch (ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().AsEnum())
        {
            case EventingSettings.MessagingServiceProviderOptionsEnum.InMemory:
                block.AddInvocationStatement("x.UsingInMemory", memory => memory
                    .AddArgument(new CSharpLambdaBlock("(context, cfg)")
                        .AddStatement(GetMessageRetryStatement("cfg", configurationVarName))
                        .AddStatements(GetPostHostConfigurationStatements()))
                    .AddMetadata("message-broker", "memory")
                    .SeparatedFromPrevious());
                break;
            case EventingSettings.MessagingServiceProviderOptionsEnum.Rabbitmq:
                block.AddInvocationStatement("x.UsingRabbitMq", rabbitMq => rabbitMq
                    .AddArgument(new CSharpLambdaBlock("(context, cfg)")
                        .AddStatement(GetMessageRetryStatement("cfg", configurationVarName))
                        .AddInvocationStatement("cfg.Host", host => host
                            .AddArgument(@"configuration[""RabbitMq:Host""]")
                            .AddArgument(@"configuration[""RabbitMq:VirtualHost""]")
                            .AddArgument(new CSharpLambdaBlock("host")
                                .AddStatement(@"host.Username(configuration[""RabbitMq:Username""]);")
                                .AddStatement(@"host.Password(configuration[""RabbitMq:Password""]);"))
                            .SeparatedFromPrevious())
                        .AddStatements(GetPostHostConfigurationStatements()))
                    .AddMetadata("message-broker", "rabbit-mq")
                    .SeparatedFromPrevious());
                break;
            case EventingSettings.MessagingServiceProviderOptionsEnum.AzureServiceBus:
                block.AddInvocationStatement("x.UsingAzureServiceBus", azBus => azBus
                    .AddArgument(new CSharpLambdaBlock("(context, cfg)")
                        .AddStatement(GetMessageRetryStatement("cfg", configurationVarName))
                        .AddInvocationStatement("cfg.Host", host => host
                            .AddArgument(@"configuration[""AzureMessageBus:ConnectionString""]")
                            .SeparatedFromPrevious())
                        .AddStatements(GetPostHostConfigurationStatements()))
                    .AddMetadata("message-broker", "azure-service-bus")
                    .SeparatedFromPrevious());
                break;
            case EventingSettings.MessagingServiceProviderOptionsEnum.AmazonSqs:
                block.AddInvocationStatement("x.UsingAmazonSqs", sqs => sqs
                    .AddArgument(new CSharpLambdaBlock("(context, cfg)")
                        .AddStatement(GetMessageRetryStatement("cfg", configurationVarName))
                        .AddInvocationStatement("cfg.Host", host => host
                            .AddArgument(@"configuration[""AmazonSqs:Host""]")
                            .AddArgument(new CSharpLambdaBlock("host")
                                .AddStatement(@"host.AccessKey(configuration[""AmazonSqs:AccessKey""]);")
                                .AddStatement(@"host.SecretKey(configuration[""AmazonSqs:SecretKey""]);"))
                            .SeparatedFromPrevious())
                        .AddStatements(GetPostHostConfigurationStatements()))
                    .AddMetadata("message-broker", "amazon-sqs")
                    .SeparatedFromPrevious());
                break;
            default:
                throw new InvalidOperationException(
                    $"Messaging Service Provider is set to a setting that is not supported: {ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().AsEnum()}");
        }

        if (ExecutionContext.Settings.GetEventingSettings().OutboxPattern().IsInMemory())
        {
            block.AddStatement("x.AddInMemoryInboxOutbox();");
        }

        return block;
    }

    private IEnumerable<CSharpStatement> GetPostHostConfigurationStatements()
    {
        yield return new CSharpStatement("cfg.ConfigureEndpoints(context);").AddMetadata("configure-endpoints", true);
        if (MessageHandlerModels.Any(HasMessageBrokerStereotype))
        {
            yield return new CSharpStatement($@"cfg.ConfigureNonDefaultEndpoints(context);");
        }

        if (ExecutionContext.Settings.GetEventingSettings().OutboxPattern().IsInMemory())
        {
            yield return new CSharpStatement("cfg.UseInMemoryOutbox();");
        }
        else if (ExecutionContext.Settings.GetEventingSettings().OutboxPattern().IsEntityFramework() &&
                 ExecutionContext.GetApplicationConfig().Modules.All(p => p.ModuleId != "Intent.Eventing.MassTransit.EntityFrameworkCore"))
        {
            Logging.Log.Warning("Please install Intent.Eventing.MassTransit.EntityFrameworkCore module for the Outbox pattern to persist to the database");
        }

        if (MessagesWithSettings.Any())
        {
            yield return new CSharpStatement("cfg.AddMessageTopologyConfiguration();");
        }

        if (ExecutionContext.Settings.GetEventingSettings().EnableScheduledPublishing())
        {
            yield return new CSharpStatement("cfg.UseDelayedMessageScheduler();");
        }
    }

    private void AddMessageTopologyConfiguration(CSharpClass @class)
    {
        if (!MessagesWithSettings.Any())
        {
            return;
        }

        @class.AddMethod("void", "AddMessageTopologyConfiguration", method =>
        {
            method.Private().Static();
            method.AddParameter(GetMessageBrokerBusFactoryConfiguratorName(), "cfg", param => param.WithThisModifier());
            foreach (var messageModel in MessagesWithSettings)
            {
                method.AddStatement($@"cfg.Message<{GetTypeName(IntegrationEventMessageTemplate.TemplateId, messageModel)}>(x => x.SetEntityName(""{messageModel.GetMessageTopologySettings().EntityName()}""));");
            }
        });
    }

    private IReadOnlyCollection<CSharpStatement> GetConsumerStatements(string configParamName)
    {
        var statements = new List<CSharpStatement>();
        foreach (var messageHandlerModel in MessageHandlerModels)
        {
            var messageName =
                this.GetIntegrationEventMessageName(messageHandlerModel.TypeReference.Element.AsMessageModel());
            var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.Replace("_", "-").Replace(" ", "-")
                .Replace(".", "-");
            var consumerDefinitionType =
                $@"{this.GetIntegrationEventHandlerInterfaceName()}<{messageName}>, {messageName}";
            var consumerWrapperType = $@"{this.GetWrapperConsumerName()}<{consumerDefinitionType}>";

            // Until we can do single-line method chaining this will have to do for now...
            var addConsumer = $@"{configParamName}.AddConsumer<{consumerWrapperType}>"
                              + $@"(typeof({this.GetWrapperConsumerName()}Definition<{consumerDefinitionType}>))";

            if (HasMessageBrokerStereotype(messageHandlerModel))
            {
                addConsumer += $@".ExcludeFromConfigureEndpoints()";
            }
            else
            {
                addConsumer += $@".Endpoint(config => config.InstanceId = ""{sanitizedAppName}"")";
            }

            addConsumer += ";";
            statements.Add(addConsumer);
        }

        return statements;
    }

    private string GetMessageBrokerBusFactoryConfiguratorName()
    {
        return ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().AsEnum() switch
        {
            EventingSettings.MessagingServiceProviderOptionsEnum.AmazonSqs => "IAmazonSqsBusFactoryConfigurator",
            EventingSettings.MessagingServiceProviderOptionsEnum.InMemory => "IInMemoryBusFactoryConfigurator",
            EventingSettings.MessagingServiceProviderOptionsEnum.Rabbitmq => "IRabbitMqBusFactoryConfigurator",
            EventingSettings.MessagingServiceProviderOptionsEnum.AzureServiceBus => "IServiceBusBusFactoryConfigurator",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private string GetMessageBrokerReceiveEndpointConfiguratorName()
    {
        return ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().AsEnum() switch
        {
            EventingSettings.MessagingServiceProviderOptionsEnum.AmazonSqs => "IAmazonSqsReceiveEndpointConfigurator",
            EventingSettings.MessagingServiceProviderOptionsEnum.InMemory => "IInMemoryReceiveEndpointConfigurator",
            EventingSettings.MessagingServiceProviderOptionsEnum.Rabbitmq => "IRabbitMqReceiveEndpointConfigurator",
            EventingSettings.MessagingServiceProviderOptionsEnum.AzureServiceBus => "IServiceBusReceiveEndpointConfigurator",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void AddNonDefaultEndpointConfigurationMethods(CSharpClass @class)
    {
        if (!MessageHandlerModels.Any(HasMessageBrokerStereotype))
        {
            return;
        }

        @class.AddMethod("void", "ConfigureNonDefaultEndpoints", method =>
        {
            method.Private().Static();
            method.AddParameter(GetMessageBrokerBusFactoryConfiguratorName(), "cfg", parm => parm.WithThisModifier());
            method.AddParameter("IBusRegistrationContext", "context");

            foreach (var messageHandlerModel in MessageHandlerModels.Where(HasMessageBrokerStereotype))
            {
                var messageName =
                    this.GetIntegrationEventMessageName(messageHandlerModel.TypeReference.Element.AsMessageModel());
                var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.Replace("_", "-").Replace(" ", "-")
                    .Replace(".", "-");
                var consumerDefinitionType =
                    $@"{this.GetIntegrationEventHandlerInterfaceName()}<{messageName}>, {messageName}";
                var consumerWrapperType = $@"{this.GetWrapperConsumerName()}<{consumerDefinitionType}>";

                method.AddInvocationStatement($"cfg.AddCustomConsumerEndpoint<{consumerWrapperType}>", inv => inv
                    .AddArgument("context")
                    .AddArgument($@"""{sanitizedAppName}""")
                    .AddArgument(new CSharpLambdaBlock("endpoint")
                        .AddStatements(AddMessageBrokerConfigurationStatements("endpoint", messageHandlerModel)))
                    .WithArgumentsOnNewLines());
            }
        });

        @class.AddMethod("void", "AddCustomConsumerEndpoint", method =>
        {
            method.Private().Static();
            method.AddGenericParameter("TConsumer", out var tConsumer);
            method.AddGenericTypeConstraint(tConsumer, c => c.AddType("class").AddType("IConsumer"));
            method.AddParameter(GetMessageBrokerBusFactoryConfiguratorName(), "cfg", parm => parm.WithThisModifier());
            method.AddParameter("IBusRegistrationContext", "context");
            method.AddParameter("string", "instanceId");
            method.AddParameter($"Action<{GetMessageBrokerReceiveEndpointConfiguratorName()}>", "configuration");

            method.AddInvocationStatement($"cfg.ReceiveEndpoint", stmt => stmt
                .AddArgument(new CSharpInvocationStatement($"new ConsumerEndpointDefinition<{tConsumer}>")
                    .WithoutSemicolon()
                    .AddArgument(new CSharpObjectInitializerBlock($@"new EndpointSettings<IEndpointDefinition<{tConsumer}>>")
                        .AddInitStatement("InstanceId", "instanceId")))
                .AddArgument("KebabCaseEndpointNameFormatter.Instance")
                .AddArgument(new CSharpLambdaBlock("endpoint")
                    .AddStatement("configuration.Invoke(endpoint);")
                    .AddStatement($"endpoint.ConfigureConsumer<{tConsumer}>(context);"))
                .WithArgumentsOnNewLines());
        });
    }

    private IEnumerable<CSharpStatement> AddMessageBrokerConfigurationStatements(string configVarName,
        MessageSubscribeAssocationTargetEndModel messageHandlerModel)
    {
        if (messageHandlerModel.HasAzureServiceBusConsumerSettings() &&
            ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().IsAzureServiceBus())
        {
            var settings = messageHandlerModel.GetAzureServiceBusConsumerSettings();
            if (settings.PrefetchCount().HasValue)
            {
                yield return $@"{configVarName}.PrefetchCount = {settings.PrefetchCount()};";
            }

            yield return $@"{configVarName}.RequiresSession = {settings.RequiresSession().ToString().ToLower()};";
            if (!string.IsNullOrWhiteSpace(settings.DefaultMessageTimeToLive()))
            {
                ValidateTimeSpanString(settings.DefaultMessageTimeToLive(), nameof(settings.DefaultMessageTimeToLive), out var ts);
                yield return $@"{configVarName}.DefaultMessageTimeToLive = TimeSpan.Parse(""{ts}"");";
            }

            if (!string.IsNullOrWhiteSpace(settings.LockDuration()))
            {
                ValidateTimeSpanString(settings.LockDuration(), nameof(settings.LockDuration), out var ts);
                yield return $@"{configVarName}.LockDuration = TimeSpan.Parse(""{ts}"");";
            }

            yield return $@"{configVarName}.RequiresDuplicateDetection = {settings.RequiresDuplicateDetection().ToString().ToLower()};";
            if (settings.RequiresDuplicateDetection() && !string.IsNullOrWhiteSpace(settings.DuplicateDetectionHistoryTimeWindow()))
            {
                ValidateTimeSpanString(settings.DuplicateDetectionHistoryTimeWindow(), nameof(settings.DuplicateDetectionHistoryTimeWindow), out var ts);
                yield return $@"{configVarName}.DuplicateDetectionHistoryTimeWindow = TimeSpan.Parse(""{ts}"");";
            }

            yield return $@"{configVarName}.EnableBatchedOperations = {settings.EnableBatchedOperations().ToString().ToLower()};";
            yield return $@"{configVarName}.EnableDeadLetteringOnMessageExpiration = {settings.EnableDeadLetteringOnMessageExpiration().ToString().ToLower()};";
            if (settings.MaxQueueSize().HasValue)
            {
                yield return $@"{configVarName}.MaxSizeInMegabytes = {settings.MaxQueueSize()};";
            }

            if (settings.MaxDeliveryCount().HasValue)
            {
                yield return $@"{configVarName}.MaxDeliveryCount = {settings.MaxDeliveryCount()};";
            }
        }

        if (messageHandlerModel.HasRabbitMQConsumerSettings() &&
            ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().IsRabbitmq())
        {
            var settings = messageHandlerModel.GetRabbitMQConsumerSettings();
            if (settings.PrefetchCount().HasValue)
            {
                yield return $@"{configVarName}.PrefetchCount = {settings.PrefetchCount()};";
            }

            yield return $@"{configVarName}.Lazy = {settings.Lazy().ToString().ToLower()};";
            yield return $@"{configVarName}.Durable = {settings.Durable().ToString().ToLower()};";
            yield return $@"{configVarName}.PurgeOnStartup = {settings.PurgeOnStartup().ToString().ToLower()};";
            yield return $@"{configVarName}.Exclusive = {settings.Exclusive().ToString().ToLower()};";
        }

        yield break;

        // Until we get a nice UI text field that can capture time this will have to do
        static void ValidateTimeSpanString(string settingStringValue, string memberName, out TimeSpan parsedTimeSpan)
        {
            if (!TimeSpan.TryParse(settingStringValue, out parsedTimeSpan))
            {
                throw new Exception($"Unable to parse '{settingStringValue}' for {memberName}. Ensure format is 'hh:mm:ss'.");
            }
        }
    }

    private bool HasMessageBrokerStereotype(MessageSubscribeAssocationTargetEndModel messageHandlerModel)
    {
        return (messageHandlerModel.HasAzureServiceBusConsumerSettings() &&
                ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().IsAzureServiceBus())
               ||
               (messageHandlerModel.HasRabbitMQConsumerSettings() &&
                ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().IsRabbitmq());
    }

    private CSharpStatement GetMessageRetryStatement(string configParamName, string configurationVarName)
    {
        return ExecutionContext.Settings.GetEventingSettings().RetryPolicy().AsEnum() switch
        {
            EventingSettings.RetryPolicyOptionsEnum.RetryImmediate => GetCSharpRetryStatements("Immediate",
                ("int", "RetryLimit", "5")),
            EventingSettings.RetryPolicyOptionsEnum.RetryInterval => GetCSharpRetryStatements("Interval",
                ("int", "RetryCount", "10"),
                ("TimeSpan", "Interval", "TimeSpan.FromSeconds(5)")),
            EventingSettings.RetryPolicyOptionsEnum.RetryIncremental => GetCSharpRetryStatements("Incremental",
                ("int", "RetryLimit", "10"),
                ("TimeSpan", "InitialInterval", "TimeSpan.FromSeconds(5)"),
                ("TimeSpan", "IntervalIncrement", "TimeSpan.FromSeconds(5)")),
            EventingSettings.RetryPolicyOptionsEnum.RetryExponential => GetCSharpRetryStatements("Exponential",
                ("int", "RetryLimit", "10"),
                ("TimeSpan", "MinInterval", "TimeSpan.FromSeconds(5)"),
                ("TimeSpan", "MaxInterval", "TimeSpan.FromMinutes(30)"),
                ("TimeSpan", "IntervalDelta", "TimeSpan.FromSeconds(5)")),
            EventingSettings.RetryPolicyOptionsEnum.RetryNone => GetCSharpRetryStatements("None"),
            _ => throw new ArgumentOutOfRangeException()
        };

        CSharpStatement GetCSharpRetryStatements(string methodName, params (string Type, string Name, string DefaultValue)[] args)
        {
            var retry = new CSharpInvocationStatement($"r.{methodName}")
                .WithoutSemicolon()
                .WithArgumentsOnNewLines();

            foreach (var arg in args)
            {
                retry.AddArgument($@"{configurationVarName}.GetValue<{arg.Type}?>(""MassTransit:Retry{methodName}:{arg.Name}"") ?? {arg.DefaultValue}");
            }

            return new CSharpInvocationStatement($@"{configParamName}.UseMessageRetry")
                .AddArgument(new CSharpLambdaBlock("r").WithExpressionBody(retry));
        }
    }

    private void PublishRetryPoliciesAppSettings()
    {
        // Justification: No, there aren't any articles that I could find on the internet
        // that would provide as good defaults, but based on the use cases for each retry
        // policy, I've put together some values that make sense.

        switch (ExecutionContext.Settings.GetEventingSettings().RetryPolicy().AsEnum())
        {
            case EventingSettings.RetryPolicyOptionsEnum.RetryImmediate:
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("MassTransit:RetryImmediate",
                    new
                    {
                        RetryLimit = 5
                    }));
                break;
            case EventingSettings.RetryPolicyOptionsEnum.RetryInterval:
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("MassTransit:RetryInterval",
                    new
                    {
                        RetryCount = 10,
                        Interval = TimeSpan.FromSeconds(5)
                    }));
                break;
            case EventingSettings.RetryPolicyOptionsEnum.RetryIncremental:
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("MassTransit:RetryIncremental",
                    new
                    {
                        RetryLimit = 10,
                        InitialInterval = TimeSpan.FromSeconds(5),
                        IntervalIncrement = TimeSpan.FromSeconds(5)
                    }));
                break;
            case EventingSettings.RetryPolicyOptionsEnum.RetryExponential:
                // I used the MassTransit algo to work out this one.
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("MassTransit:RetryExponential",
                    new
                    {
                        RetryLimit = 10,
                        MinInterval = TimeSpan.FromSeconds(5),
                        MaxInterval = TimeSpan.FromMinutes(30),
                        IntervalDelta = TimeSpan.FromSeconds(5)
                    }));
                break;
            case EventingSettings.RetryPolicyOptionsEnum.RetryNone:
            default:
                break;
        }
    }

    public override void BeforeTemplateExecution()
    {
        ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest.ToRegister(
                "AddMassTransitConfiguration",
                ServiceConfigurationRequest.ParameterType.Configuration)
            .ForConcern("Infrastructure")
            .HasDependency(this));

        PublishRetryPoliciesAppSettings();

        switch (ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().AsEnum())
        {
            case EventingSettings.MessagingServiceProviderOptionsEnum.InMemory:
                // InMemory doesn't require appsettings
                break;
            case EventingSettings.MessagingServiceProviderOptionsEnum.Rabbitmq:
                AddNugetDependency(NuGetPackages.MassTransitRabbitMq);

                ExecutionContext.EventDispatcher.Publish(
                    new AppSettingRegistrationRequest("RabbitMq", new
                    {
                        Host = "localhost",
                        VirtualHost = "/",
                        Username = "guest",
                        Password = "guest"
                    }));

                break;
            case EventingSettings.MessagingServiceProviderOptionsEnum.AzureServiceBus:
                AddNugetDependency(NuGetPackages.MassTransitAzureServiceBusCore);

                ExecutionContext.EventDispatcher.Publish(
                    new AppSettingRegistrationRequest("AzureMessageBus", new
                    {
                        ConnectionString = "your connection string"
                    }));
                break;
            case EventingSettings.MessagingServiceProviderOptionsEnum.AmazonSqs:
                AddNugetDependency(NuGetPackages.MassTransitAmazonSqs);

                ExecutionContext.EventDispatcher.Publish(
                    new AppSettingRegistrationRequest("AmazonSqs", new
                    {
                        Host = "us-east-1",
                        AccessKey = "your-iam-access-key",
                        SecretKey = "your-iam-secret-key"
                    }));
                break;
            default:
                throw new InvalidOperationException(
                    $"Messaging Service Provider is set to a setting that is not supported: {ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().AsEnum()}");
        }
    }

    [IntentManaged(Mode.Fully)]
    public CSharpFile CSharpFile { get; }

    [IntentManaged(Mode.Fully)]
    protected override CSharpFileConfig DefineFileConfig()
    {
        return CSharpFile.GetConfig();
    }

    [IntentManaged(Mode.Fully)]
    public override string TransformText()
    {
        return CSharpFile.ToString();
    }
}