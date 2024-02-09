﻿using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.DocumentDB.Shared;

namespace Intent.Modules.Redis.Om.Repositories.FactoryExtensions;

public class RedisOmPrimaryKeyInitStrategy : IPrimaryKeyInitStrategy
{
    public bool ShouldInsertPkInitializationCode(ClassModel targetClass)
    {
        return true;
    }

    public string GetGetterInitExpression(ICSharpTemplate template, ClassModel targetClass, string fieldName, string fieldTypeName)
    {
        return fieldTypeName switch
        {
            "string" => targetClass.IsAggregateRoot() ? fieldName : $"{fieldName} ??= {template.UseType("System.Guid")}.NewGuid().ToString()",
            "guid" => $"{fieldName} ??= {template.UseType("System.Guid")}.NewGuid()",
            "int" or "long" => $"{fieldName} ?? throw new {template.UseType("System.NullReferenceException")}(\"{fieldName} has not been set\")",
            _ => fieldName
        };
    }
}