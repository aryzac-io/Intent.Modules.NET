using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.OpenTelemetry.OpenTelemetryConfiguration", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Api.Configuration
{
    public static class OpenTelemetryConfiguration
    {
        public static IServiceCollection AddTelemetryConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(res => res
                    .AddService("Standard.AspNetCore.TestApplication")
                    .AddTelemetrySdk()
                    .AddEnvironmentVariableDetector())
                .WithTracing(trace => trace
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter());
            return services;
        }

        public static ILoggingBuilder AddTelemetryConfiguration(
            this ILoggingBuilder logBuilder,
            HostBuilderContext context)
        {
            return logBuilder.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(ResourceBuilder
                    .CreateDefault()
                    .AddService("Standard.AspNetCore.TestApplication"));
                options.AddConsoleExporter();
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                options.ParseStateValues = true;
            });
        }
    }
}