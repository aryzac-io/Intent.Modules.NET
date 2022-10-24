﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Eventing.MassTransit.Templates.WrapperConsumer
{
    using Intent.Modelers.Eventing.Api;
    using Intent.Modules.Common.CSharp.Templates;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit\Templates\WrapperConsumer\WrapperConsumerTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class WrapperConsumerTemplate : CSharpTemplateBase<object, Intent.Modules.Eventing.MassTransit.Templates.WrapperConsumer.ConsumerDecorator>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing System.Threading.Tasks;\r\nusing MassTransit;\r\nusing Microsoft.Extensions.DependencyInjection;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 12 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit\Templates\WrapperConsumer\WrapperConsumerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 14 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit\Templates\WrapperConsumer\WrapperConsumerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("<THandler, TMessage> : IConsumer<TMessage>\r\n        where TMessage : class\r\n        where THandler : ");
            
            #line 16 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit\Templates\WrapperConsumer\WrapperConsumerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetIntegrationEventHandlerInterfaceName()));
            
            #line default
            #line hidden
            this.Write("<TMessage>\r\n    {");
            
            #line 17 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit\Templates\WrapperConsumer\WrapperConsumerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassMembers()));
            
            #line default
            #line hidden
            this.Write("\r\n        public ");
            
            #line 18 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit\Templates\WrapperConsumer\WrapperConsumerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 18 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit\Templates\WrapperConsumer\WrapperConsumerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetConstructorParameters()));
            
            #line default
            #line hidden
            this.Write(")\r\n        {");
            
            #line 19 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit\Templates\WrapperConsumer\WrapperConsumerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetConstructorImplementation()));
            
            #line default
            #line hidden
            this.Write("\r\n        }\r\n\r\n        public async Task Consume(ConsumeContext<TMessage> context)\r\n        {\r\n            var eventBus = _serviceProvider.GetService<");
            
            #line 24 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit\Templates\WrapperConsumer\WrapperConsumerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetMassTransitEventBusName()));
            
            #line default
            #line hidden
            this.Write(">()");
            
            #line 24 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit\Templates\WrapperConsumer\WrapperConsumerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(UseExplicitNullSymbol ? "!" : string.Empty));
            
            #line default
            #line hidden
            this.Write(";\r\n            eventBus.Current = context;\r\n");
            
            #line 26 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit\Templates\WrapperConsumer\WrapperConsumerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetConsumeEnterCode()));
            
            #line default
            #line hidden
            this.Write("\r\n            var handler = _serviceProvider.GetService<THandler>()");
            
            #line 27 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit\Templates\WrapperConsumer\WrapperConsumerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(UseExplicitNullSymbol ? "!" : string.Empty));
            
            #line default
            #line hidden
            this.Write(";            \r\n            await handler.HandleAsync(context.Message, context.CancellationToken);");
            
            #line 28 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit\Templates\WrapperConsumer\WrapperConsumerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetConsumeExitCode()));
            
            #line default
            #line hidden
            this.Write("\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
}
