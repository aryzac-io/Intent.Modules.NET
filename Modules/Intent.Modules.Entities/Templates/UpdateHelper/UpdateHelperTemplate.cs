// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Entities.Templates.UpdateHelper
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
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Entities\Templates\UpdateHelper\UpdateHelperTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class UpdateHelperTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing System.Collections.Generic;\r\n\r\n[assembly: DefaultIntentManag" +
                    "ed(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 15 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Entities\Templates\UpdateHelper\UpdateHelperTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public static class ");
            
            #line 17 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Entities\Templates\UpdateHelper\UpdateHelperTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(@"
    {
        public static ICollection<TOriginal> CreateOrUpdateCollection<TChanged, TOriginal>(
            ICollection<TOriginal> baseCollection,
            ICollection<TChanged> changedCollection,
            Func<TOriginal, TChanged, bool> equalityCheck,
            Func<TOriginal, TChanged, TOriginal> assignmentAction)
            where TOriginal : class, new()
        {

            baseCollection ??= new List<TOriginal>()!;

            var result = baseCollection.CompareCollections(changedCollection, equalityCheck);
            foreach (var elementToAdd in result.ToAdd)
            {
                var newEntity = new TOriginal();
                assignmentAction(newEntity, elementToAdd);

                baseCollection.Add(newEntity);
            }

            foreach (var elementToRemove in result.ToRemove)
            {
                baseCollection.Remove(elementToRemove);
            }

            foreach (var elementToEdit in result.PossibleEdits)
            {
                assignmentAction(elementToEdit.Original, elementToEdit.Changed);
            }

            return baseCollection;
        }
    }
}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
