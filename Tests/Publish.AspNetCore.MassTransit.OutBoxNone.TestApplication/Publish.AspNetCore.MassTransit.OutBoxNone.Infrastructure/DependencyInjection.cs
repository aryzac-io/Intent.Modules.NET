using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Publish.AspNetCore.MassTransit.OutBoxNone.Application.Common.Eventing;
using Publish.AspNetCore.MassTransit.OutBoxNone.Domain.Common.Interfaces;
using Publish.AspNetCore.MassTransit.OutBoxNone.Infrastructure.Configuration;
using Publish.AspNetCore.MassTransit.OutBoxNone.Infrastructure.Eventing;
using Publish.AspNetCore.MassTransit.OutBoxNone.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxNone.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetService<ApplicationDbContext>());
            services.AddScoped<MassTransitEventBus>();
            services.AddTransient<IEventBus>(provider => provider.GetService<MassTransitEventBus>());
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}