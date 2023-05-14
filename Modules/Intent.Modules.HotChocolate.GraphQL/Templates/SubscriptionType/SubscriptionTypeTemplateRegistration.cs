using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Modelers.Services.GraphQL.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.Templates.SubscriptionType
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class SubscriptionTypeTemplateRegistration : FilePerModelTemplateRegistration<GraphQLSubscriptionTypeModel>
    {
        private readonly IMetadataManager _metadataManager;

        public SubscriptionTypeTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => SubscriptionTypeTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, GraphQLSubscriptionTypeModel model)
        {
            return new SubscriptionTypeTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<GraphQLSubscriptionTypeModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetGraphQLSubscriptionTypeModels();
        }
    }
}