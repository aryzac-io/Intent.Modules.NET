﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.AspNetCore.WebApi.Templates.Controller
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
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class WebApiControllerTemplate : CSharpTemplateBase<ServiceModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(" \r\n");
            this.Write(@" 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
");
            
            #line 25 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DependencyUsings));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 29 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    [Route(\"");
            
            #line 31 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetRoute()));
            
            #line default
            #line hidden
            this.Write("\")]\r\n    public class ");
            
            #line 32 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : Controller\r\n    {\r\n        private readonly ");
            
            #line 34 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetServiceInterfaceName()));
            
            #line default
            #line hidden
            this.Write(" _appService;");
            
            #line 34 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DeclarePrivateVariables()));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n        public ");
            
            #line 36 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" (");
            
            #line 36 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetServiceInterfaceName()));
            
            #line default
            #line hidden
            this.Write(" appService");
            
            #line 36 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ConstructorParams()));
            
            #line default
            #line hidden
            this.Write("\r\n            )\r\n        {\r\n             _appService = appService ?? throw new Ar" +
                    "gumentNullException(nameof(appService));");
            
            #line 39 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ConstructorInit()));
            
            #line default
            #line hidden
            this.Write("\r\n        } \r\n    \r\n");
            
            #line 42 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
  foreach (var operation in Model.Operations)
    {
            
            #line default
            #line hidden
            this.Write("        [Http");
            
            #line 44 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetHttpVerb(operation).ToString().ToLower().ToPascalCase()));
            
            #line default
            #line hidden
            this.Write("(\"");
            
            #line 44 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetPath(operation)));
            
            #line default
            #line hidden
            this.Write("\")]\r\n        ");
            
            #line 45 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetSecurityAttribute(operation)));
            
            #line default
            #line hidden
            this.Write("\r\n        [ProducesResponseType(");
            
            #line 46 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(operation.ReturnType != null ? string.Format("typeof({0}), ", GetOperationReturnType(operation)) : ""));
            
            #line default
            #line hidden
            this.Write("(int)HttpStatusCode.OK)]\r\n        public ");
            
            #line 47 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(operation.IsAsync() ? "async Task<IActionResult>" : "IActionResult"));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 47 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(operation.Name));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 47 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetOperationParameters(operation)));
            
            #line default
            #line hidden
            this.Write(")\r\n        {");
            
            #line 48 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(BeginOperation(operation)));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 49 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
      if (operation.ReturnType != null)
        {

            
            #line default
            #line hidden
            this.Write("            ");
            
            #line 51 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetOperationReturnType(operation)));
            
            #line default
            #line hidden
            this.Write(" result = default(");
            
            #line 51 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetOperationReturnType(operation)));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 52 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
      }
            
            #line default
            #line hidden
            this.Write("            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.Re" +
                    "adCommitted };\r\n\r\n            try\r\n            {");
            
            #line 56 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(BeforeTransaction(operation)));
            
            #line default
            #line hidden
            this.Write("\r\n                using (TransactionScope ts = new TransactionScope(TransactionSc" +
                    "opeOption.Required, tso");
            
            #line 57 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(operation.IsAsync() ? ", TransactionScopeAsyncFlowOption.Enabled" : ""));
            
            #line default
            #line hidden
            this.Write("))\r\n                {");
            
            #line 58 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(BeforeCallToAppLayer(operation)));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 59 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
      if (operation.ReturnType != null) {
            
            #line default
            #line hidden
            this.Write("                    var appServiceResult = ");
            
            #line 60 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(operation.IsAsync() ? "await " : ""));
            
            #line default
            #line hidden
            this.Write("_appService.");
            
            #line 60 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(operation.Name));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 60 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetOperationCallParameters(operation)));
            
            #line default
            #line hidden
            this.Write(");\r\n                    result = appServiceResult;\r\n");
            
            #line 62 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
      } else { 
            
            #line default
            #line hidden
            this.Write("                    ");
            
            #line 63 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(operation.IsAsync() ? "await " : ""));
            
            #line default
            #line hidden
            this.Write("_appService.");
            
            #line 63 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(operation.Name));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 63 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetOperationCallParameters(operation)));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 64 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
      } 
            
            #line default
            #line hidden
            
            #line 64 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(AfterCallToAppLayer(operation)));
            
            #line default
            #line hidden
            this.Write("\r\n                    ts.Complete();\r\n                }");
            
            #line 66 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(AfterTransaction(operation)));
            
            #line default
            #line hidden
            this.Write("\r\n            }\r\n            catch (Exception e)\r\n            {");
            
            #line 69 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(OnExceptionCaught(operation)));
            
            #line default
            #line hidden
            this.Write("\r\n            }\r\n\r\n");
            
            #line 72 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
      if (operation.ReturnType != null)
        {
            
            #line default
            #line hidden
            this.Write("            return Ok(result);\r\n");
            
            #line 75 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
      } else {
            
            #line default
            #line hidden
            this.Write("            return Ok();\r\n");
            
            #line 77 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
      }
            
            #line default
            #line hidden
            this.Write("\r\n        }\r\n\r\n");
            
            #line 81 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
    }

        // Source code of base class: http://aspnetwebstack.codeplex.com/SourceControl/latest#src/System.Web.Http/ApiController.cs
        // As dispose is not virtual, looking at the source code, this looks like a better hook in point

            
            #line default
            #line hidden
            this.Write("        protected override void Dispose(bool disposing)\r\n        {\r\n            b" +
                    "ase.Dispose(disposing);\r\n\r\n            //dispose all resources\r\n            _app" +
                    "Service.Dispose();");
            
            #line 91 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(OnDispose()));
            
            #line default
            #line hidden
            this.Write("\r\n        }\r\n");
            
            #line 93 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.WebApi\Templates\Controller\WebApiControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassMethods()));
            
            #line default
            #line hidden
            this.Write("\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
