using System;
using Integration.HttpClients.TestApplication.Application.InvoiceProxy;
using Integration.HttpClients.TestApplication.Application.MultiVersionServiceProxy;
using Integration.HttpClients.TestApplication.Application.VersionOneServiceProxy;
using Integration.HttpClients.TestApplication.Infrastructure.HttpClients;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "1.0")]

namespace Integration.HttpClients.TestApplication.Infrastructure.Configuration
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAccessTokenManagement(options =>
            {
                configuration.GetSection("IdentityClients").Bind(options.Client.Clients);
            }).ConfigureBackchannelHttpClient();

            services.AddHttpClient<IInvoiceProxyClient, InvoiceProxyHttpClient>(http =>
            {
                http.BaseAddress = configuration.GetValue<Uri>("HttpClients:InvoiceProxy:Uri");
                http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:InvoiceProxy:Timeout") ?? TimeSpan.FromSeconds(100);
            }).AddClientAccessTokenHandler(configuration.GetValue<string>("HttpClients:InvoiceProxy:IdentityClientKey") ?? "default");
            services.AddHttpClient<IMultiVersionServiceProxyClient, MultiVersionServiceProxyHttpClient>(http =>
            {
                http.BaseAddress = configuration.GetValue<Uri>("HttpClients:MultiVersionServiceProxy:Uri");
                http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:MultiVersionServiceProxy:Timeout") ?? TimeSpan.FromSeconds(100);
            });
            services.AddHttpClient<IVersionOneServiceProxyClient, VersionOneServiceProxyHttpClient>(http =>
            {
                http.BaseAddress = configuration.GetValue<Uri>("HttpClients:VersionOneServiceProxy:Uri");
                http.Timeout = configuration.GetValue<TimeSpan?>("HttpClients:VersionOneServiceProxy:Timeout") ?? TimeSpan.FromSeconds(100);
            });
        }
    }
}