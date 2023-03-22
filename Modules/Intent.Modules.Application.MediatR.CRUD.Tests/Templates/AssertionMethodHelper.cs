﻿using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates;

public static class AssertionMethodHelper
{
    public static void AddAssertionMethods(this ICSharpFileBuilderTemplate template, CSharpClass builderClass, CommandModel commandModel, ClassModel domainModel, bool skipIdFields = true)
    {
        builderClass.AddMethod("void", GetAssertionMethodName(domainModel.InternalElement), method =>
        {
            method.Static();
            method.AddParameter(template.GetTypeName(commandModel.InternalElement), "expectedDto");
            method.AddParameter(template.GetTypeName(domainModel.InternalElement), "actualEntity");
            method.AddStatements(GetDtoToDomainPropertyAssignments(template, "actualEntity", "expectedDto", domainModel, commandModel.Properties, skipIdFields));
        });
    }

    public static void AddAssertionMethods(this ICSharpFileBuilderTemplate template, CSharpClass builderClass, ClassModel domainModel, DTOModel dtoModel, bool hasCollection)
    {
        builderClass.AddMethod("void", GetAssertionMethodName(domainModel.InternalElement), method =>
        {
            method.Static();

            var dtoModelTypeName = hasCollection ? $"IEnumerable<{template.GetTypeName(dtoModel.InternalElement)}>" : template.GetTypeName(dtoModel.InternalElement);
            var domainModelTypeName = hasCollection ? $"IEnumerable<{template.GetTypeName(domainModel.InternalElement)}>" : template.GetTypeName(domainModel.InternalElement);
            method.AddParameter(dtoModelTypeName, "actualDto");
            method.AddParameter(domainModelTypeName, "expectedEntity");
            method.AddStatements(GetDomainToDtoPropertyAssignments(template, "actualDto", "expectedEntity", dtoModel, domainModel));
        });
    }

    private static IEnumerable<CSharpStatement> GetDtoToDomainPropertyAssignments(
        ICSharpFileBuilderTemplate template, 
        string actualEntityVarName, 
        string expectedDtoVarName, 
        ClassModel domainModel,
        IList<DTOFieldModel> dtoFields,
        bool skipIdFields)
    {
        var codeLines = new List<CSharpStatement>();

        codeLines.Add(new CSharpStatementBlock($"if ({expectedDtoVarName} == null)")
            .AddStatements($@"
            {actualEntityVarName}.Should().BeNull();
            return;"));
        
        codeLines.Add(string.Empty);
        
        codeLines.Add($"{actualEntityVarName}.Should().NotBeNull();");
        
        foreach (var field in dtoFields)
        {
            // Implicit ID case check
            if (!skipIdFields &&
                field.Mapping?.Element == null &&
                field.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
            {
                codeLines.Add($"{actualEntityVarName}.Id.Should().Be({expectedDtoVarName}.{field.Name.ToPascalCase()});");
                continue;
            }

            // Implicit Owner ID case check
            ClassModel ownerEntity;
            if (field.Mapping?.Element == null &&
                (ownerEntity = domainModel.GetNestedCompositionalOwner()) != null &&
                field.Name.Equals($"{ownerEntity.Name}id", StringComparison.OrdinalIgnoreCase))
            {
                codeLines.Add($"{actualEntityVarName}.Id.Should().Be({expectedDtoVarName}.{field.Name.ToPascalCase()});");
                continue;
            }

            if (field.Mapping?.Element == null && 
                domainModel.Attributes.All(p => p.Name != field.Name))
            {
                continue;
            }

            switch (field.Mapping?.Element?.SpecializationTypeId)
            {
                default:
                case AttributeModel.SpecializationTypeId:
                    var attribute = field.Mapping?.Element?.AsAttributeModel()
                                    ?? domainModel.Attributes.First(p => p.Name == field.Name);
                    if (!skipIdFields || !attribute.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        codeLines.Add($"{actualEntityVarName}.{attribute.Name.ToPascalCase()}.Should().Be({expectedDtoVarName}.{field.Name.ToPascalCase()});");
                        break;
                    }

                    break;
                case AssociationTargetEndModel.SpecializationTypeId:
                {
                    var association = field.Mapping.Element.AsAssociationTargetEndModel();
                    if (association.Association.AssociationType == AssociationType.Aggregation)
                    {
                        codeLines.Add($@"#warning Field not a composite association: {field.Name.ToPascalCase()}");
                        break;
                    }
                    
                    var targetType = association.Element.AsClassModel();
                    var attributeName = association.Name.ToPascalCase();
                    var fieldDtoModel = field.TypeReference.Element.AsDTOModel();

                    codeLines.Add($"AssertEquivalent({expectedDtoVarName}.{field.Name.ToPascalCase()}, {actualEntityVarName}.{attributeName});");

                    var @class = template.CSharpFile.Classes.First();

                    var dtoParamType = template.GetTypeName(field.TypeReference);
                    var entityParamType = template.GetTypeName(field.Mapping.Element.TypeReference);
                    if (@class.FindMethod(stmt => stmt.Parameters.First().Type == dtoParamType && stmt.Parameters.Last().Type == entityParamType) != null)
                    {
                        continue;
                    }

                    if (field.TypeReference.IsCollection)
                    {
                        @class.AddMethod("void", GetAssertionMethodName(targetType.InternalElement, attributeName),
                            method => method.Static()
                                .AddParameter(dtoParamType, "expectedDtos")
                                .AddParameter(entityParamType, "actualEntities")
                                .AddStatementBlock($"if (expectedDtos == null)", block => block
                                    .AddStatements($@"
            actualEntities.Should().BeNullOrEmpty();
            return;"))
                                .AddStatement(string.Empty)
                                .AddStatement("actualEntities.Should().HaveSameCount(actualEntities);")
                                .AddStatementBlock("for (int i = 0; i < expectedDtos.Count(); i++)", block => block
                                    .AddStatements($@"
            var dto = expectedDtos.ElementAt(i);
            var entity = actualEntities.ElementAt(i);")
                                    .AddStatements(GetDtoToDomainPropertyAssignments(template, "entity", "dto", targetType, fieldDtoModel.Fields, skipIdFields)))
                        );
                    }
                    else
                    {
                        @class.AddMethod("void", GetAssertionMethodName(targetType.InternalElement, attributeName),
                            method => method.Static()
                                .AddParameter(dtoParamType, "expectedDto")
                                .AddParameter(entityParamType, "actualEntity")
                                .AddStatements(GetDtoToDomainPropertyAssignments(template, "actualEntity", "expectedDto", targetType, fieldDtoModel.Fields, skipIdFields)));
                    }
                }
                    break;
            }
        }

        return codeLines;
    }
    
   private static IEnumerable<CSharpStatement> GetDomainToDtoPropertyAssignments(
        ICSharpFileBuilderTemplate template, 
        string actualDtoVarName, 
        string expectedEntityVarName, 
        DTOModel dtoModel,
        ClassModel domainModel)
    {
        var codeLines = new List<CSharpStatement>();
        
        codeLines.Add(new CSharpStatementBlock($"if ({expectedEntityVarName} == null)")
            .AddStatements($@"
            {actualDtoVarName}.Should().BeNull();
            return;"));
        
        codeLines.Add(string.Empty);
        
        codeLines.Add($"{actualDtoVarName}.Should().NotBeNull();");
        
        foreach (var field in dtoModel.Fields)
        {
            // Implicit ID case check
            if (field.Mapping?.Element == null &&
                field.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
            {
                codeLines.Add($"{actualDtoVarName}.{field.Name.ToPascalCase()}.Should().Be({expectedEntityVarName}.Id);");
                continue;
            }
            
            // Implicit Owner ID case check
            ClassModel ownerEntity;
            if (field.Mapping?.Element == null &&
                (ownerEntity = domainModel.GetNestedCompositionalOwner()) != null &&
                field.Name.Equals($"{ownerEntity.Name}id", StringComparison.OrdinalIgnoreCase))
            {
                codeLines.Add($"{actualDtoVarName}.{field.Name.ToPascalCase()}.Should().Be({expectedEntityVarName}.{ownerEntity.Name}Id);");
                continue;
            }
            
            if (field.Mapping?.Element == null
                && domainModel.Attributes.All(p => p.Name != field.Name))
            {
                continue;
            }
            
            switch (field.Mapping?.Element?.SpecializationTypeId)
            {
                default:
                case AttributeModel.SpecializationTypeId:
                    var attribute = field.Mapping?.Element?.AsAttributeModel()
                                    ?? domainModel.Attributes.First(p => p.Name == field.Name);
                    codeLines.Add($"{actualDtoVarName}.{field.Name.ToPascalCase()}.Should().Be({expectedEntityVarName}.{attribute.Name.ToPascalCase()});");

                    break;
                case AssociationTargetEndModel.SpecializationTypeId:
                {
                    var association = field.Mapping.Element.AsAssociationTargetEndModel();
                    var targetType = association.Element.AsClassModel();
                    var attributeName = association.Name.ToPascalCase();
                    var fieldDtoModel = field.TypeReference.Element.AsDTOModel();

                    codeLines.Add($"AssertEquivalent({actualDtoVarName}.{field.Name.ToPascalCase()}, {expectedEntityVarName}.{attributeName});");
                    
                    var @class = template.CSharpFile.Classes.First();
                    
                    var dtoParamType = template.GetTypeName(field.TypeReference);
                    var entityParamType = template.GetTypeName(field.Mapping.Element.TypeReference);
                    if (@class.FindMethod(stmt => stmt.Parameters.First().Type == dtoParamType && stmt.Parameters.Last().Type == entityParamType) != null)
                    {
                        continue;
                    }
                    
                    if (field.TypeReference.IsCollection)
                    {
                        @class.AddMethod("void", GetAssertionMethodName(targetType.InternalElement, attributeName),
                            method => method.Static()
                                .AddParameter(dtoParamType, "actualDtos")
                                .AddParameter(entityParamType, "expectedEntities")
                                .AddStatementBlock($"if (expectedEntities == null)", block => block
                                    .AddStatements($@"
            actualDtos.Should().BeNullOrEmpty();
            return;"))
                                .AddStatement(string.Empty)
                                .AddStatement("actualDtos.Should().HaveSameCount(actualDtos);")
                                .AddStatementBlock("for (int i = 0; i < expectedEntities.Count(); i++)", block => block
                                    .AddStatements($@"
            var entity = expectedEntities.ElementAt(i);
            var dto = actualDtos.ElementAt(i);")
                                    .AddStatements(GetDomainToDtoPropertyAssignments(template, "dto", "entity", fieldDtoModel, targetType)))
                        );
                    }
                    else
                    {
                        @class.AddMethod("void", GetAssertionMethodName(targetType.InternalElement, attributeName),
                            method => method.Static()
                                .AddParameter(dtoParamType, "actualDto")
                                .AddParameter(entityParamType, "expectedEntity")
                                .AddStatements(GetDomainToDtoPropertyAssignments(template, "actualDto", "expectedEntity", fieldDtoModel, targetType)));
                    }
                }
                    break;
            }
        }

        return codeLines;
    }

    private static string GetAssertionMethodName(IElement targetType, string attributeName = "")
    {
        return $"AssertEquivalent";
    }
}