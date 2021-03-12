// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.AspNetCore.Templates.Startup
{
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore\Templates\Startup\StartupTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class StartupTemplate : CSharpTemplateBase<object, Intent.Modules.AspNetCore.Templates.Startup.StartupDecorator>
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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
");
            
            #line 24 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore\Templates\Startup\StartupTemplate.tt"
  if (!IsNetCore2App()) { 
            
            #line default
            #line hidden
            this.Write("using Microsoft.Extensions.Hosting;\r\n");
            
            #line 26 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore\Templates\Startup\StartupTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 30 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore\Templates\Startup\StartupTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    [IntentManaged(Mode.Merge)]\r\n    public class ");
            
            #line 33 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore\Templates\Startup\StartupTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        public ");
            
            #line 35 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore\Templates\Startup\StartupTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(IConfiguration configuration)\r\n        {\r\n            Configuration = configurat" +
                    "ion;\r\n        }\r\n\r\n        public IConfiguration Configuration { get; }\r\n\r\n     " +
                    "   public void ConfigureServices(IServiceCollection services)\r\n        {\r\n");
            
            #line 44 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore\Templates\Startup\StartupTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetServiceConfigurations("            ")));
            
            #line default
            #line hidden
            
            #line 44 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore\Templates\Startup\StartupTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Registrations()));
            
            #line default
            #line hidden
            this.Write("\r\n        }\r\n\r\n        // This method gets called by the runtime. Use this method" +
                    " to configure the HTTP request pipeline.\r\n        public void Configure(IApplica" +
                    "tionBuilder app, ");
            
            #line 48 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore\Templates\Startup\StartupTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture((IsNetCore2App() ? "IHostingEnvironment" : "IWebHostEnvironment")));
            
            #line default
            #line hidden
            this.Write(" env)\r\n        {\r\n            if (env.IsDevelopment())\r\n            {\r\n          " +
                    "      app.UseDeveloperExceptionPage();\r\n            }\r\n");
            
            #line 54 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore\Templates\Startup\StartupTemplate.tt"
  if (IsNetCore2App()) { 
            
            #line default
            #line hidden
            this.Write("            else\r\n            {\r\n                // The default HSTS value is 30 " +
                    "days. You may want to change this for production scenarios, see https://aka.ms/a" +
                    "spnetcore-hsts.\r\n                app.UseHsts();\r\n            }\r\n");
            
            #line 60 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore\Templates\Startup\StartupTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 62 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore\Templates\Startup\StartupTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetApplicationConfigurations("            ")));
            
            #line default
            #line hidden
            this.Write("\r\n        }");
            
            #line 63 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore\Templates\Startup\StartupTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetDecoratorsOutput(x => x.Methods())));
            
            #line default
            #line hidden
            this.Write("\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
