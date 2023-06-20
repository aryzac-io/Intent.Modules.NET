using System.Reflection;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Behaviours;
using CleanArchitecture.TestApplication.Application.Implementation.ServiceDispatch;
using CleanArchitecture.TestApplication.Application.Interfaces.ServiceDispatch;
using CleanArchitecture.TestApplication.Domain.Services;
using CleanArchitecture.TestApplication.Domain.Services.Async;
using CleanArchitecture.TestApplication.Domain.Services.DDD;
using CleanArchitecture.TestApplication.Domain.Services.DefaultDiagram;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehaviour<,>));
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IAccountingDomainService, AccountingDomainService>();
            services.AddTransient<IAsyncableDomainService, AsyncableDomainService>();
            services.AddTransient<IDataContractDomainService, DataContractDomainService>();
            services.AddTransient<IDomainServiceWithDefault, DomainServiceWithDefault>();
            services.AddTransient<IServiceDispatchService, ServiceDispatchService>();
            return services;
        }
    }
}