using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Decorators
{
    [Description(DefaultControllerServicesStartupDecorator.DecoratorId)]
    public class DefaultControllerServicesStartupDecoratorRegistration : DecoratorRegistration<StartupTemplate, StartupDecorator>
    {
        public override StartupDecorator CreateDecoratorInstance(StartupTemplate template, IApplication application)
        {
            return new DefaultControllerServicesStartupDecorator(template, application);
        }

        public override string DecoratorId => DefaultControllerServicesStartupDecorator.DecoratorId;
    }
}