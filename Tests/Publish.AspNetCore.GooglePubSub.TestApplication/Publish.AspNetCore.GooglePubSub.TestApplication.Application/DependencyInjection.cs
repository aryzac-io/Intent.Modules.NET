using System.Reflection;
using AutoMapper;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Publish.AspNetCore.GooglePubSub.TestApplication.Application.Common.Eventing;
using Publish.AspNetCore.GooglePubSub.TestApplication.Application.Common.Validation;
using Publish.AspNetCore.GooglePubSub.TestApplication.Application.Implementation;
using Publish.AspNetCore.GooglePubSub.TestApplication.Application.IntegrationEventHandlers;
using Publish.AspNetCore.GooglePubSub.TestApplication.Application.IntegrationEvents;
using Publish.AspNetCore.GooglePubSub.TestApplication.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Publish.AspNetCore.GooglePubSub.TestApplication.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient<IPublishService, PublishService>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IIntegrationEventHandler<GenericMessage>, GenericEventHandler>();
            return services;
        }
    }
}