// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration
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
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore\Templates\EntityTypeConfiguration\EntityTypeConfigurationTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class EntityTypeConfigurationTemplate : CSharpTemplateBase<Intent.Modelers.Domain.Api.ClassModel, Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration.EntityTypeConfigurationDecorator>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing Microsoft.EntityFrameworkCore;\r\nusing Microsoft.EntityFramew" +
                    "orkCore.Metadata.Builders;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nna" +
                    "mespace ");
            
            #line 16 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore\Templates\EntityTypeConfiguration\EntityTypeConfigurationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 18 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore\Templates\EntityTypeConfiguration\EntityTypeConfigurationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : IEntityTypeConfiguration<");
            
            #line 18 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore\Templates\EntityTypeConfiguration\EntityTypeConfigurationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetEntityName()));
            
            #line default
            #line hidden
            this.Write(">\r\n    {");
            
            #line 19 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore\Templates\EntityTypeConfiguration\EntityTypeConfigurationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassMembers()));
            
            #line default
            #line hidden
            
            #line 19 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore\Templates\EntityTypeConfiguration\EntityTypeConfigurationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetConstructor()));
            
            #line default
            #line hidden
            this.Write("\r\n        public void Configure(EntityTypeBuilder<");
            
            #line 20 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore\Templates\EntityTypeConfiguration\EntityTypeConfigurationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetEntityName()));
            
            #line default
            #line hidden
            this.Write("> builder)\r\n        {");
            
            #line 21 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore\Templates\EntityTypeConfiguration\EntityTypeConfigurationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetTypeConfiguration()));
            
            #line default
            #line hidden
            this.Write("\r\n        }");
            
            #line 22 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore\Templates\EntityTypeConfiguration\EntityTypeConfigurationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetAdditionalMethods()));
            
            #line default
            #line hidden
            this.Write("\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
