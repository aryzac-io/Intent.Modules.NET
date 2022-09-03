using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Modules.Entities.Templates.DomainEnum;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Entities.Templates.DomainEntityInterface
{
    [IntentManaged(Mode.Ignore, Body = Mode.Merge)]
    partial class DomainEntityInterfaceTemplate : CSharpTemplateBase<ClassModel>, ITemplate, ITemplatePostCreationHook, IDeclareUsings, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Entities.DomainEntityInterface";
        private readonly IMetadataManager _metadataManager;
        public const string Identifier = "Intent.Entities.DomainEntityInterface";
        public CSharpFile CSharpFile { get; set; }

        //private readonly IList<DomainEntityInterfaceDecoratorBase> _decorators = new List<DomainEntityInterfaceDecoratorBase>();

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public DomainEntityInterfaceTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            _metadataManager = ExecutionContext.MetadataManager;
            if (!ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
            {
                SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateICollection());
            }
            AddTypeSource(TemplateId);
            AddTypeSource(DomainEnumTemplate.TemplateId);
            AddTypeSource("Domain.ValueObject");
        }

        public override void OnCreated()
        {
            if (Model.Operations.Any(x => x.IsAsync()))
            {
                AddUsing("System.Threading.Tasks");
            }

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"I{Model.Name}", @interface =>
                {
                    if (Model.ParentClass != null)
                    {
                        @interface.ExtendsInterface(this.GetDomainEntityInterfaceName(Model.ParentClass));
                    }

                    foreach (var attribute in Model.Attributes)
                    {
                        @interface.AddProperty(GetTypeName(attribute), attribute.Name.ToPascalCase(), property =>
                        {
                            property.AddMetadata("model", attribute);
                            if (ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
                            {
                                property.ReadOnly();
                            }
                        });
                    }

                    foreach (var associationEnd in Model.AssociatedClasses.Where(x => x.IsNavigable))
                    {
                        @interface.AddProperty(GetTypeName(associationEnd), associationEnd.Name.ToPascalCase(), property =>
                        {
                            property.AddMetadata("model", associationEnd);
                            property.Virtual();
                            if (ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
                            {
                                property.ReadOnly();
                            }
                        });
                    }

                    foreach (var operation in Model.Operations)
                    {
                        @interface.AddMethod(GetOperationReturnType(operation), operation.Name, method =>
                        {
                            foreach (var parameter in operation.Parameters)
                            {
                                method.AddParameter(GetTypeName(parameter), parameter.Name);
                            }
                        });
                    }
                });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"I{Model.Name}",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        [IntentManaged(Mode.Ignore, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        //public void AddDecorator(DomainEntityInterfaceDecoratorBase decorator)
        //{
        //    _decorators.Add(decorator);
        //}

        //public IEnumerable<DomainEntityInterfaceDecoratorBase> GetDecorators()
        //{
        //    return _decorators.OrderBy(x => x.Priority);
        //}

        //public string GetInterfaces(ClassModel @class)
        //{
        //    var interfaces = GetDecorators().SelectMany(x => x.GetInterfaces(@class)).Distinct().ToList();
        //    if (Model.GetStereotypeProperty("Base Type", "Has Interface", false) && GetBaseTypeInterface() != null)
        //    {
        //        interfaces.Insert(0, GetBaseTypeInterface());
        //    }

        //    return string.Join(", ", interfaces);
        //}

        private string GetBaseTypeInterface()
        {
            var typeId = Model.GetStereotypeProperty<string>("Base Type", "Type");
            if (typeId == null)
            {
                return null;
            }


            // GCB - There is definitely a better way to handle this now (V3.0)
            var type = _metadataManager.Domain(OutputTarget.Application).GetTypeDefinitionModels().FirstOrDefault(x => x.Id == typeId);
            if (type != null)
            {
                return $"I{type.Name}";
            }
            throw new Exception($"Could not find Base Type for class {Model.Name}");
        }

        //public string InterfaceAnnotations(ClassModel @class)
        //{
        //    return GetDecorators().Aggregate(x => x.InterfaceAnnotations(@class));
        //}

        //public string BeforeProperties(ClassModel @class)
        //{
        //    return GetDecorators().Aggregate(x => x.BeforeProperties(@class));
        //}

        //public string PropertyBefore(AttributeModel attribute)
        //{
        //    return GetDecorators().Aggregate(x => x.PropertyBefore(attribute));
        //}

        //public string PropertyAnnotations(AttributeModel attribute)
        //{
        //    return GetDecorators().Aggregate(x => x.PropertyAnnotations(attribute));
        //}

        //public string PropertyBefore(AssociationEndModel associationEnd)
        //{
        //    return GetDecorators().Aggregate(x => x.PropertyBefore(associationEnd));
        //}

        //public string PropertyAnnotations(AssociationEndModel associationEnd)
        //{
        //    return GetDecorators().Aggregate(x => x.PropertyAnnotations(associationEnd));
        //}

        //public string AttributeAccessors(AttributeModel attribute)
        //{
        //    return GetDecorators().Select(x => x.AttributeAccessors(attribute)).FirstOrDefault(x => x != null) ?? "get; set;";
        //}

        //public string AssociationAccessors(AssociationEndModel associationEnd)
        //{
        //    return GetDecorators().Select(x => x.AssociationAccessors(associationEnd)).FirstOrDefault(x => x != null) ?? "get; set;";
        //}

        //public bool CanWriteDefaultAttribute(AttributeModel attribute)
        //{
        //    return GetDecorators().All(x => x.CanWriteDefaultAttribute(attribute));
        //}

        //public bool CanWriteDefaultAssociation(AssociationEndModel association)
        //{
        //    return GetDecorators().All(x => x.CanWriteDefaultAssociation(association));
        //}

        //public bool CanWriteDefaultOperation(OperationModel operation)
        //{
        //    return GetDecorators().All(x => x.CanWriteDefaultOperation(operation));
        //}


        public string GetOperationReturnType(OperationModel o)
        {
            if (o.TypeReference.Element == null)
            {
                return o.IsAsync() ? "Task" : "void";
            }
            return o.IsAsync() ? $"Task<{GetTypeName(o.TypeReference)}>" : GetTypeName(o.TypeReference);
        }
    }
}
