// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Application.MediatR.Behaviours.Templates.EventBusPublishBehaviour
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR.Behaviours\Templates\EventBusPublishBehaviour\EventBusPublishBehaviourTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class EventBusPublishBehaviourTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System.Threading;\r\nusing System.Threading.Tasks;\r\nusing MediatR;\r\n\r\n[assemb" +
                    "ly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 16 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR.Behaviours\Templates\EventBusPublishBehaviour\EventBusPublishBehaviourTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write(";\r\n\r\npublic class ");
            
            #line 18 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR.Behaviours\Templates\EventBusPublishBehaviour\EventBusPublishBehaviourTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>\r\nwhere TRequest : " +
                    "notnull\r\n{\r\n    private readonly ");
            
            #line 21 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR.Behaviours\Templates\EventBusPublishBehaviour\EventBusPublishBehaviourTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetEventBusInterfaceName()));
            
            #line default
            #line hidden
            this.Write(" _eventBus;\r\n\r\n    public ");
            
            #line 23 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR.Behaviours\Templates\EventBusPublishBehaviour\EventBusPublishBehaviourTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 23 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR.Behaviours\Templates\EventBusPublishBehaviour\EventBusPublishBehaviourTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetEventBusInterfaceName()));
            
            #line default
            #line hidden
            this.Write(@" eventBus)
    {
        _eventBus = eventBus;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        await _eventBus.FlushAllAsync(cancellationToken);

        return response;
    }
}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
