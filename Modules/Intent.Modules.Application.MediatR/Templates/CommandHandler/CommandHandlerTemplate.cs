// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Application.MediatR.Templates.CommandHandler
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using Intent.Modelers.Services.CQRS.Api;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR\Templates\CommandHandler\CommandHandlerTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class CommandHandlerTemplate : CSharpTemplateBase<Intent.Modelers.Services.CQRS.Api.CommandModel, Intent.Modules.Application.MediatR.Templates.CommandHandler.CommandHandlerDecorator>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using MediatR;\r\nusing System;\r\nusing System.Collections.Generic;\r\nusing System.Li" +
                    "nq;\r\nusing System.Threading;\r\nusing System.Threading.Tasks;\r\n\r\n[assembly: Defaul" +
                    "tIntentManaged(Mode.Merge)]\r\n\r\nnamespace ");
            
            #line 20 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR\Templates\CommandHandler\CommandHandlerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]\r\n    public class ");
            
            #line 23 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR\Templates\CommandHandler\CommandHandlerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 23 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR\Templates\CommandHandler\CommandHandlerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetRequestHandlerInterface()));
            
            #line default
            #line hidden
            this.Write("\r\n    {");
            
            #line 24 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR\Templates\CommandHandler\CommandHandlerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetFields()));
            
            #line default
            #line hidden
            this.Write("\r\n        [IntentInitialGen]\r\n        public ");
            
            #line 26 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR\Templates\CommandHandler\CommandHandlerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 26 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR\Templates\CommandHandler\CommandHandlerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetCtorParams()));
            
            #line default
            #line hidden
            this.Write(")\r\n        {");
            
            #line 27 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR\Templates\CommandHandler\CommandHandlerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetCtorInitializations()));
            
            #line default
            #line hidden
            this.Write("\r\n        }\r\n\r\n        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]\r\n     " +
                    "   public async Task<");
            
            #line 31 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR\Templates\CommandHandler\CommandHandlerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetReturnType()));
            
            #line default
            #line hidden
            this.Write("> Handle(");
            
            #line 31 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR\Templates\CommandHandler\CommandHandlerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetCommandModelName()));
            
            #line default
            #line hidden
            this.Write(" request, CancellationToken cancellationToken)\r\n        {");
            
            #line 32 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR\Templates\CommandHandler\CommandHandlerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetImplementation()));
            
            #line default
            #line hidden
            this.Write("\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
