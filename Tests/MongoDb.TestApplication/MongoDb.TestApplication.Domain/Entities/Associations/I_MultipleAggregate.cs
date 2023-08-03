using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class I_MultipleAggregate
    {
        [IntentManaged(Mode.Fully)]
        public I_MultipleAggregate()
        {
            Id = null!;
            Attribute = null!;
            JRequireddependentId = null!;
        }
        public string Id { get; set; }

        public string Attribute { get; set; }

        public string JRequireddependentId { get; set; }
    }
}