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
            this.Write("\r\n    {\r\n        public static TOriginal UpdateObject<TChanged, TOriginal>(\r\n    " +
                    "        this TOriginal baseElement,\r\n            TChanged changedElement,\r\n     " +
                    "       Action<TOriginal, TChanged> assignmentAction)\r\n        {\r\n            if " +
                    "(baseElement == null)\r\n            {\r\n                return baseElement;\r\n     " +
                    "       }\r\n\r\n            assignmentAction(baseElement, changedElement);\r\n        " +
                    "    return baseElement;\r\n        }\r\n        \r\n        public static void UpdateC" +
                    "ollection<TChanged, TOriginal>(\r\n            this ICollection<TOriginal> baseCol" +
                    "lection, \r\n            ICollection<TChanged> changedCollection,\r\n            Fun" +
                    "c<TOriginal, TChanged, bool> equalityCheck,\r\n            Action<TOriginal, TChan" +
                    "ged> assignmentAction)\r\n            where TOriginal: class, new()\r\n        {\r\n  " +
                    "          if (changedCollection == null)\r\n            {\r\n                baseCol" +
                    "lection.Clear();\r\n                return;\r\n            }\r\n            \r\n        " +
                    "    var result = baseCollection.CompareCollections(changedCollection, equalityCh" +
                    "eck);\r\n            foreach (var elementToAdd in result.ToAdd)\r\n            {\r\n  " +
                    "              var newEntity = new TOriginal();\r\n                assignmentAction" +
                    "(newEntity, elementToAdd);\r\n                \r\n                baseCollection.Add" +
                    "(newEntity);\r\n            }\r\n            \r\n            foreach (var elementToRem" +
                    "ove in result.ToRemove)\r\n            {\r\n                baseCollection.Remove(el" +
                    "ementToRemove);\r\n            }\r\n            \r\n            foreach (var elementTo" +
                    "Edit in result.PossibleEdits)\r\n            {\r\n                assignmentAction(e" +
                    "lementToEdit.Original, elementToEdit.Changed);\r\n            }\r\n        }\r\n    }\r" +
                    "\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
