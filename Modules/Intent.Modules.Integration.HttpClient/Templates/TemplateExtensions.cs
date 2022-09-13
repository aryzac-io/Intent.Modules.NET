using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClient.Templates.RequestHttpException;
using Intent.Modules.Integration.HttpClient.Templates.ServiceProxiesConfiguration;
using Intent.Modules.Integration.HttpClient.Templates.ServiceProxyClient;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClient.Templates
{
    public static class TemplateExtensions
    {
        public static string GetRequestHttpExceptionName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(RequestHttpExceptionTemplate.TemplateId);
        }
        public static string GetServiceProxiesConfigurationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(ServiceProxiesConfigurationTemplate.TemplateId);
        }

        public static string GetServiceProxyClientName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Types.ServiceProxies.Api.ServiceProxyModel
        {
            return template.GetTypeName(ServiceProxyClientTemplate.TemplateId, template.Model);
        }

        public static string GetServiceProxyClientName(this IntentTemplateBase template, Intent.Modelers.Types.ServiceProxies.Api.ServiceProxyModel model)
        {
            return template.GetTypeName(ServiceProxyClientTemplate.TemplateId, model);
        }

    }
}