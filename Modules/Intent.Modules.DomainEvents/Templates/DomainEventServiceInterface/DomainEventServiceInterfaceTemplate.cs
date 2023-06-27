// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.DomainEvents.Templates.DomainEventServiceInterface
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
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.DomainEvents\Templates\DomainEventServiceInterface\DomainEventServiceInterfaceTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class DomainEventServiceInterfaceTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System.Threading;\r\nusing System.Threading.Tasks;\r\n\r\n[assembly: DefaultInten" +
                    "tManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 15 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.DomainEvents\Templates\DomainEventServiceInterface\DomainEventServiceInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public interface ");
            
            #line 17 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.DomainEvents\Templates\DomainEventServiceInterface\DomainEventServiceInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        Task Publish(");
            
            #line 19 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.DomainEvents\Templates\DomainEventServiceInterface\DomainEventServiceInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetDomainEventBaseType()));
            
            #line default
            #line hidden
            this.Write(" domainEvent, CancellationToken cancellationToken = default);\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
