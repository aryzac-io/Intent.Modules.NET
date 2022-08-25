﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Eventing.MassTransit.MediatR.Templates.MessageBusPublishBehaviour
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using Intent.Modules.Eventing.MassTransit.Templates;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit.MediatR\Templates\MessageBusPublishBehaviour\MessageBusPublishBehaviourTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class MessageBusPublishBehaviourTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System.Threading;\r\nusing System.Threading.Tasks;\r\nusing MediatR;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 17 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit.MediatR\Templates\MessageBusPublishBehaviour\MessageBusPublishBehaviourTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 19 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit.MediatR\Templates\MessageBusPublishBehaviour\MessageBusPublishBehaviourTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>\r\n    where TRequest : IRequest<TResponse>\r\n    {\r\n        private readonly ");
            
            #line 22 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit.MediatR\Templates\MessageBusPublishBehaviour\MessageBusPublishBehaviourTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetMessageBufferInterfaceName()));
            
            #line default
            #line hidden
            this.Write(" _messageBuffer;\r\n\r\n        public ");
            
            #line 24 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit.MediatR\Templates\MessageBusPublishBehaviour\MessageBusPublishBehaviourTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 24 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.MassTransit.MediatR\Templates\MessageBusPublishBehaviour\MessageBusPublishBehaviourTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetMessageBufferInterfaceName()));
            
            #line default
            #line hidden
            this.Write(" messageBuffer)\r\n        {\r\n            _messageBuffer = messageBuffer;\r\n        }\r\n        \r\n        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)\r\n        {\r\n            var response = await next();\r\n\r\n            await _messageBuffer.FlushAllAsync(cancellationToken);\r\n\r\n            return response;\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
}
