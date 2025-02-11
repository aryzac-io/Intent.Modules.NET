using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.WrapperConsumer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public abstract class ConsumerDecorator : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;
    }
}