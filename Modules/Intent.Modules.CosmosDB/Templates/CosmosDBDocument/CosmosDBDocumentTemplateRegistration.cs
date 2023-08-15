using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates.CosmosDBDocument
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CosmosDBDocumentTemplateRegistration : FilePerModelTemplateRegistration<ClassModel>
    {
        private readonly IMetadataManager _metadataManager;

        public CosmosDBDocumentTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => CosmosDBDocumentTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ClassModel model)
        {
            return new CosmosDBDocumentTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ClassModel> GetModels(IApplication application)
        {
            return _metadataManager.Domain(application).GetClassModels()
                .Where(x => CosmosDbProvider.FilterDbProvider(x) &&
                            x.IsAggregateRoot() && !x.IsAbstract)
                .ToArray();
        }
    }
}