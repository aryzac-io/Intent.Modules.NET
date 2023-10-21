using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class SingleIndexEntityMultiParent
    {
        public string Id { get; set; }

        public string SomeField { get; set; }

        public ICollection<SingleIndexEntityMultiChild> SingleIndexEntityMultiChild { get; set; } = new List<SingleIndexEntityMultiChild>();
    }
}