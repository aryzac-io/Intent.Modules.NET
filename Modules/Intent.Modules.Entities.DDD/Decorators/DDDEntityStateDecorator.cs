﻿using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Plugins;

namespace Intent.Modules.Entities.DDD.Decorators
{
    public class DDDEntityStateDecorator : DomainEntityStateDecoratorBase, ISupportsConfiguration
    {
        public const string Identifier = "Intent.Entities.DDD.EntityStateDecorator";

        private const string AggregateRootBaseClassSetting = "Aggregate Root Base Class";

        private const string EntityBaseClassSetting = "Entity Base Class";

        private const string ValueObjectBaseClassSetting = "Value Object Base Class";

        private string _aggregateRootBaseClass;

        private string _entityBaseClass;

        private string _valueObjectBaseClass;

        public DDDEntityStateDecorator(DomainEntityStateTemplate template) : base(template)
        {
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);
            if (settings.ContainsKey(AggregateRootBaseClassSetting) && !string.IsNullOrWhiteSpace(settings[AggregateRootBaseClassSetting]))
            {
                _aggregateRootBaseClass = settings[AggregateRootBaseClassSetting];
            }
            if (settings.ContainsKey(EntityBaseClassSetting) && !string.IsNullOrWhiteSpace(settings[EntityBaseClassSetting]))
            {
                _entityBaseClass = settings[EntityBaseClassSetting];
            }
            if (settings.ContainsKey(ValueObjectBaseClassSetting) && !string.IsNullOrWhiteSpace(settings[ValueObjectBaseClassSetting]))
            {
                _valueObjectBaseClass = settings[ValueObjectBaseClassSetting];
            }
        }

        public override string ConvertAttributeType(AttributeModel attribute)
        {
            var domainType = attribute.GetStereotypeProperty<string>("DomainType", "Type");
            if (domainType != null)
            {
                return domainType;
            }
            return base.ConvertAttributeType(attribute);
        }

        public override string GetBaseClass(ClassModel @class)
        {
            var baseClass = @class.GetStereotypeProperty<string>("Aggregate Root", "BaseType");
            if (baseClass != null)
            {
                return baseClass;
            }
            baseClass = @class.GetStereotypeProperty<string>("Entity", "BaseType");
            if (baseClass != null)
            {
                return baseClass;
            }
            baseClass = @class.GetStereotypeProperty<string>("Value Object", "BaseType");
            if (baseClass != null)
            {
                return baseClass;
            }
            if (_aggregateRootBaseClass != null && @class.Stereotypes.FirstOrDefault(s => s.Name == "Aggregate Root") != null)
            {
                return _aggregateRootBaseClass;
            }
            if (_entityBaseClass != null && @class.Stereotypes.FirstOrDefault(s => s.Name == "Entity") != null)
            {
                return _entityBaseClass;
            }
            if (_valueObjectBaseClass != null && @class.Stereotypes.FirstOrDefault(s => s.Name == "Value Object") != null)
            {
                return _valueObjectBaseClass;
            }
            return base.GetBaseClass(@class);
        }

        public override string AssociationAfter(AssociationEndModel associationEnd)
        {
            if (!associationEnd.IsNavigable)
            {
                return base.AssociationAfter(associationEnd);
            }

            var typeInfo = Template.Types.InContext(DomainEntityStateTemplate.InterfaceContext).Get(associationEnd);

            return $@"
        {Template.UseType(typeInfo)} I{associationEnd.OtherEnd().Class.Name}.{associationEnd.Name().ToPascalCase()} => {associationEnd.Name().ToPascalCase()};
";
        }
    }
}
