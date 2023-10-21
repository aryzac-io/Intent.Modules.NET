using System;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.NestedComposition
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class ClassC
    {
        public Guid Id { get; set; }

        public string ClassCAttr { get; set; }
    }
}