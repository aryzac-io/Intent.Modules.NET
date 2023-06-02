using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Modelers.Domain.Settings;
using OperationModelExtensions = Intent.Modelers.Domain.Api.OperationModelExtensions;
using ParameterModelExtensions = Intent.Modelers.Domain.Api.ParameterModelExtensions;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    public class UpdateImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;
        private readonly Lazy<StrategyData> _matchingElementDetails;

        public UpdateImplementationStrategy(CommandHandlerTemplate template)
        {
            _template = template;
            _matchingElementDetails = new Lazy<StrategyData>(GetMatchingElementDetails);
        }

        internal StrategyData GetStrategyData() => _matchingElementDetails.Value;

        public bool IsMatch()
        {
            return _matchingElementDetails.Value.IsMatch;
        }

        public void ApplyStrategy()
        {
            var @class = _template.CSharpFile.Classes.First();
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);
            _template.AddUsing("System.Linq");
            var ctor = @class.Constructors.First();
            var repository = _matchingElementDetails.Value.Repository;
            ctor.AddParameter(repository.Type, repository.Name.ToParameterName(), param => param.IntroduceReadonlyField());

            var handleMethod = @class.FindMethod("Handle");
            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
            handleMethod.AddStatements(GetImplementation());
            if (_matchingElementDetails.Value.DtoToReturn != null)
            {
                ctor.AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper", param => param.IntroduceReadonlyField());
            }
        }

        public IEnumerable<CSharpStatement> GetImplementation()
        {
            var foundEntity = _matchingElementDetails.Value.FoundEntity;
            var repository = _matchingElementDetails.Value.Repository;
            var idField = _matchingElementDetails.Value.IdField;

            var codeLines = new List<CSharpStatement>();
            var nestedCompOwner = _matchingElementDetails.Value.FoundEntity.GetNestedCompositionalOwner();
            if (nestedCompOwner != null)
            {
                var aggregateRootField = _template.Model.Properties.GetNestedCompositionalOwnerIdField(nestedCompOwner);
                if (aggregateRootField == null)
                {
                    throw new Exception($"Nested Compositional Entity {foundEntity.Name} doesn't have an Id that refers to its owning Entity {nestedCompOwner.Name}.");
                }

                codeLines.Add($"var aggregateRoot = await {repository.FieldName}.FindByIdAsync(request.{aggregateRootField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}, cancellationToken);");
                codeLines.Add($"if (aggregateRoot == null)");
                codeLines.Add(new CSharpStatementBlock()
                    .AddStatement($@"throw new InvalidOperationException($""{{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{{request.{aggregateRootField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}}}' could not be found"");"));

                var association = nestedCompOwner.GetNestedCompositeAssociation(_matchingElementDetails.Value.FoundEntity);

                codeLines.Add($@"var element = aggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}.FirstOrDefault(p => p.{_matchingElementDetails.Value.FoundEntity.GetEntityIdAttribute(_template.ExecutionContext).IdName} == request.{idField.Name.ToPascalCase()});");
                codeLines.Add($"if (element == null)");
                codeLines.Add(new CSharpStatementBlock()
                    .AddStatement($@"throw new InvalidOperationException($""{{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, foundEntity)})}} of Id '{{request.{idField.Name.ToPascalCase()}}}' could not be found associated with {{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{{request.{aggregateRootField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}}}'"");"));
                codeLines.AddRange(GetDtoPropertyAssignments(entityVarName: "element", dtoVarName: "request", domainModel: foundEntity, dtoFields: _template.Model.Properties.Where(FilterForAnaemicMapping).ToList(), skipIdField: true));

                codeLines.Add("return Unit.Value;");
                return codeLines;
            }

            codeLines.Add($"var existing{foundEntity.Name} = await {repository.FieldName}.FindByIdAsync(request.{idField.Name.ToPascalCase()}, cancellationToken);");
            codeLines.AddRange(GetDtoPropertyAssignments(entityVarName: $"existing{foundEntity.Name}", dtoVarName: "request", domainModel: foundEntity, dtoFields: _template.Model.Properties, skipIdField: true));

            if (_template.TryGetTemplate<ICSharpFileBuilderTemplate>(
                    TemplateFulfillingRoles.Repository.Interface.Entity,
                    _matchingElementDetails.Value.RepositoryInterfaceModel,
                    out var repositoryInterfaceTemplate) &&
                repositoryInterfaceTemplate.CSharpFile.Interfaces[0].TryGetMetadata<bool>("requires-explicit-update", out var requiresUpdate) &&
                requiresUpdate)
            {
                codeLines.Add(new CSharpStatement($"{repository.FieldName}.Update(existing{foundEntity.Name});").SeparatedFromPrevious());
            }

            var dtoToReturn = _matchingElementDetails.Value.DtoToReturn;
            codeLines.Add(dtoToReturn != null
                ? $@"return existing{foundEntity.Name}.MapTo{_template.GetDtoName(dtoToReturn)}(_mapper);"
                : $"return Unit.Value;");

            return codeLines;

            bool FilterForAnaemicMapping(DTOFieldModel field)
            {
                return field.Mapping?.Element == null ||
                       field.Mapping.Element.IsAttributeModel() ||
                       field.Mapping.Element.IsAssociationEndModel();
            }
        }

        private StrategyData GetMatchingElementDetails()
        {
            if (_template.ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
            {
                return NoMatch;
            }

            var commandNameLowercase = _template.Model.Name.ToLower();
            if ((!commandNameLowercase.StartsWith("update") &&
                 !commandNameLowercase.StartsWith("edit"))
                || _template.Model.Mapping?.Element.IsClassModel() != true)
            {
                return NoMatch;
            }

            var foundEntity = _template.Model.Mapping.Element.AsClassModel();

            var idField = _template.Model.Properties.GetEntityIdField(foundEntity);
            if (idField == null)
            {
                return NoMatch;
            }

            var nestedCompOwner = foundEntity.GetNestedCompositionalOwner();
            var repositoryInterfaceModel = nestedCompOwner != null ? nestedCompOwner : foundEntity;

            var repositoryInterface = _template.GetEntityRepositoryInterfaceName(repositoryInterfaceModel);
            if (repositoryInterface == null)
            {
                return NoMatch;
            }

            var repository = new RequiredService(type: repositoryInterface,
                name: repositoryInterface.Substring(1).ToCamelCase());

            var dtoToReturn = _template.Model.TypeReference.Element?.AsDTOModel();

            return new StrategyData(true, foundEntity, idField, repository, dtoToReturn, repositoryInterfaceModel);
        }

        private IList<CSharpStatement> GetDtoPropertyAssignments(string entityVarName, string dtoVarName, ClassModel domainModel, IList<DTOFieldModel> dtoFields, bool skipIdField)
        {
            var codeLines = new CSharpStatementAggregator();
            foreach (var field in dtoFields)
            {
                if (skipIdField && field.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (field.Mapping?.Element == null
                    && domainModel.Attributes.All(p => p.Name != field.Name))
                {
                    codeLines.Add($"#warning No matching field found for {field.Name}");
                    continue;
                }

                var entityVarExpr = !string.IsNullOrWhiteSpace(entityVarName) ? $"{entityVarName}." : string.Empty;
                var fieldIsNullable = field.TypeReference.IsNullable;

                switch (field.Mapping?.Element?.SpecializationTypeId)
                {
                    default:
                        var mappedPropertyName = field.Mapping?.Element?.Name ?? "<null>";
                        codeLines.Add($"#warning No matching type for Domain: {mappedPropertyName} and DTO: {field.Name}");
                        break;
                    case null:
                    case AttributeModel.SpecializationTypeId:
                        var attribute = field.Mapping?.Element?.AsAttributeModel()
                                        ?? domainModel.Attributes.First(p => p.Name == field.Name);
                        var toListExpression = field.TypeReference.IsCollection
                            ? fieldIsNullable ? "?.ToList()" : ".ToList()"
                            : string.Empty;
                        codeLines.Add($"{entityVarExpr}{attribute.Name.ToPascalCase()} = {dtoVarName}.{field.Name.ToPascalCase()}{toListExpression};");
                        break;
                    case AssociationTargetEndModel.SpecializationTypeId:
                        {
                            var association = field.Mapping.Element.AsAssociationTargetEndModel();
                            var targetEntity = association.Element.AsClassModel();
                            var attributeName = association.Name.ToPascalCase();

                            if (association.Association.AssociationType == AssociationType.Aggregation)
                            {
                                codeLines.Add($@"#warning Field not a composite association: {field.Name.ToPascalCase()}");
                                break;
                            }

                            var property = $"{entityVarExpr}{attributeName}";
                            var updateMethodName = $"CreateOrUpdate{targetEntity.InternalElement.Name.ToPascalCase()}";

                            if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                            {
                                codeLines.Add($"{property} = {updateMethodName}({property}, {dtoVarName}.{field.Name.ToPascalCase()});");
                            }
                            else
                            {
                                var targetDto = field.TypeReference.Element.AsDTOModel();
                                codeLines.Add($"{property} = {_template.GetTypeName("Domain.Common.UpdateHelper")}.CreateOrUpdateCollection({property}, {dtoVarName}.{field.Name.ToPascalCase()}, (e, d) => e.{targetEntity.GetEntityIdAttribute(_template.ExecutionContext).IdName} == d.{targetDto.Fields.GetEntityIdField(targetEntity).Name.ToPascalCase()}, {updateMethodName});");
                            }

                            var entityTypeName = _template.GetTypeName(targetEntity.InternalElement);
                            var @class = _template.CSharpFile.Classes.First();

                            var existingMethod = @class.FindMethod(x => x.Name == updateMethodName &&
                                                                        x.ReturnType == entityTypeName &&
                                                                        x.Parameters.FirstOrDefault()?.Type == entityTypeName &&
                                                                        x.Parameters.Skip(1).FirstOrDefault()?.Type == _template.GetTypeName((IElement)field.TypeReference.Element));
                            if (existingMethod == null)
                            {
                                var nullable = fieldIsNullable ? "?" : string.Empty;

                                @class.AddMethod($"{entityTypeName}{nullable}",
                                    updateMethodName,
                                    method =>
                                    {

                                        method.Private()
                                            .Static()
                                            .AddAttribute(CSharpIntentManagedAttribute.Fully())
                                            .AddParameter($"{entityTypeName}{nullable}", "entity")
                                            .AddParameter($"{_template.GetTypeName((IElement)field.TypeReference.Element)}{nullable}", "dto");

                                        if (fieldIsNullable)
                                        {
                                            method.AddIfStatement("dto == null", s => s
                                                .AddStatement("return null;"));
                                        }

                                        method.AddStatement($"entity ??= new {entityTypeName}();", s => s.SeparatedFromPrevious())
                                            .AddStatements(GetDtoPropertyAssignments(
                                                entityVarName: "entity",
                                                dtoVarName: "dto",
                                                domainModel: targetEntity,
                                                dtoFields: field.TypeReference.Element.AsDTOModel().Fields,
                                                skipIdField: true))
                                            .AddStatement("return entity;", s => s.SeparatedFromPrevious());
                                    });
                            }
                        }
                        break;
                }
            }

            return codeLines.ToList();
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null, null, null, null);

        internal class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, DTOFieldModel idField, RequiredService repository, DTOModel dtoToReturn, ClassModel repositoryInterfaceModel)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                IdField = idField;
                Repository = repository;
                DtoToReturn = dtoToReturn;
                RepositoryInterfaceModel = repositoryInterfaceModel;
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public DTOFieldModel IdField { get; }
            public RequiredService Repository { get; }
            public DTOModel DtoToReturn { get; }
            public ClassModel RepositoryInterfaceModel { get; }
        }
    }
}