using System;
using Intent.RoslynWeaver.Attributes;

namespace MongoDb.TestApplication.Domain.Entities.Collections.FolderCollection
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class FolderCollectionEntityA
    {
        public string Id { get; set; }

        public string Attribute { get; set; }
    }
}