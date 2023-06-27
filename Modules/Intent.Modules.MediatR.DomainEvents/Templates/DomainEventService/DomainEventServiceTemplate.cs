// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.MediatR.DomainEvents.Templates.DomainEventService
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
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\DomainEventService\DomainEventServiceTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class DomainEventServiceTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using MediatR;\r\nusing Microsoft.Extensions.Logging;\r\nusing System;\r\nusing System." +
                    "Threading;\r\nusing System.Threading.Tasks;\r\n\r\n[assembly: DefaultIntentManaged(Mod" +
                    "e.Fully)]\r\n\r\nnamespace ");
            
            #line 18 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\DomainEventService\DomainEventServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 20 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\DomainEventService\DomainEventServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 20 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\DomainEventService\DomainEventServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetInterfaceType()));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        private readonly ILogger<");
            
            #line 22 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\DomainEventService\DomainEventServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("> _logger;\r\n        private readonly IPublisher _mediator;\r\n\r\n        public ");
            
            #line 25 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\DomainEventService\DomainEventServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(ILogger<");
            
            #line 25 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\DomainEventService\DomainEventServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("> logger, IPublisher mediator)\r\n        {\r\n            _logger = logger;\r\n       " +
                    "     _mediator = mediator;\r\n        }\r\n\r\n        public async Task Publish(");
            
            #line 31 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\DomainEventService\DomainEventServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetDomainEventBaseType()));
            
            #line default
            #line hidden
            this.Write(@" domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(""Publishing domain event. Event - {event}"", domainEvent.GetType().Name);
            await _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent), cancellationToken);
        }

        private INotification GetNotificationCorrespondingToDomainEvent(");
            
            #line 37 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\DomainEventService\DomainEventServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetDomainEventBaseType()));
            
            #line default
            #line hidden
            this.Write(" domainEvent)\r\n        {\r\n            var result = Activator.CreateInstance(\r\n   " +
                    "             typeof(");
            
            #line 40 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\DomainEventService\DomainEventServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetDomainEventNotificationType()));
            
            #line default
            #line hidden
            this.Write(@"<>).MakeGenericType(domainEvent.GetType()), domainEvent);
            if (result == null)
                throw new Exception($""Unable to create DomainEventNotification<{domainEvent.GetType().Name}>"");

            return (INotification)result;
        }
    }
}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
