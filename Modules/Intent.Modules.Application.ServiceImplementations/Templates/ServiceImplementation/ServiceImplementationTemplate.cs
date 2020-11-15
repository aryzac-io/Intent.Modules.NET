﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Modules.Application.Contracts;
    using Intent.Modelers.Services.Api;
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class ServiceImplementationTemplate : CSharpTemplateBase<ServiceModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(" \r\n");
            this.Write(" \r\n");
            
            #line 16 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"


            
            #line default
            #line hidden
            this.Write("\r\nusing System;\r\nusing System.Linq;\r\nusing System.Collections.Generic;\r\nusing Sys" +
                    "tem.Threading.Tasks;\r\nusing System.Threading.Tasks;\r\nusing Intent.RoslynWeaver.A" +
                    "ttributes;\r\n");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DependencyUsings));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n[assembly: DefaultIntentManaged(Mode.Merge)]\r\n\r\nnamespace ");
            
            #line 29 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 31 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 31 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(NormalizeNamespace(GetServiceInterfaceName())));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n");
            
            #line 33 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
	var parameters = GetConstructorDependencies(); 
	foreach (var parameter in parameters)
	{ 
            
            #line default
            #line hidden
            this.Write("\t\tprivate ");
            
            #line 36 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(NormalizeNamespace(parameter.ParameterType)));
            
            #line default
            #line hidden
            this.Write(" _");
            
            #line 36 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(parameter.ParameterName));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 37 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
	} 
            
            #line default
            #line hidden
            this.Write("\r\n\t\t[IntentInitialGen]\r\n        public ");
            
            #line 40 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 40 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(string.Join(", ", parameters.Select(s => string.Format("{0} {1}", NormalizeNamespace(s.ParameterType), s.ParameterName)))));
            
            #line default
            #line hidden
            this.Write(")\r\n        {\r\n");
            
            #line 42 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
	foreach (var parameter in parameters)
	{ 
            
            #line default
            #line hidden
            this.Write("\t\t\t_");
            
            #line 44 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(parameter.ParameterName));
            
            #line default
            #line hidden
            this.Write(" = ");
            
            #line 44 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(parameter.ParameterName));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 45 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
	} 
            
            #line default
            #line hidden
            this.Write("        }\r\n\r\n");
            
            #line 48 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
  foreach (var o in Model.Operations) 
	{ 
            
            #line default
            #line hidden
            this.Write("        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]\r\n" +
                    "        public ");
            
            #line 51 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetOperationReturnType(o)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 51 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(o.Name));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 51 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetOperationDefinitionParameters(o)));
            
            #line default
            #line hidden
            this.Write(")\r\n        {");
            
            #line 52 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetImplementation(o)));
            
            #line default
            #line hidden
            this.Write("\t\t\r\n        }\r\n\r\n");
            
            #line 55 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceImplementations\Templates\ServiceImplementation\ServiceImplementationTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("        public void Dispose()\r\n        {\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
