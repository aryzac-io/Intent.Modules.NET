﻿using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public class DefaultCSharpMapping : CSharpMappingBase
{
    public DefaultCSharpMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
    {
    }

    public DefaultCSharpMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
    {
    }
}

public abstract class CSharpMappingBase : ICSharpMapping
{
    protected Dictionary<ICanBeReferencedType, string> _fromReplacements = new();
    protected Dictionary<ICanBeReferencedType, string> _toReplacements = new();

    public ICanBeReferencedType Model { get; }
    public IList<ICSharpMapping> Children { get; }
    public IElementToElementMappingConnection Mapping { get; set; }
    protected readonly ICSharpFileBuilderTemplate Template;

    protected CSharpMappingBase(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template)
    {
        Template = template;
        Model = model;
        Mapping = mapping;
        Children = children.Select(x => x.GetMapping()).ToList();
    }

    protected CSharpMappingBase(MappingModel model, ICSharpFileBuilderTemplate template)
    {
        Template = template;
        Model = model.Model;
        Mapping = model.Mapping;
        Children = model.Children.Select(x => x.GetMapping()).ToList();
    }

    //protected CSharpMappingBase(MappingModel mappingModel, ICSharpFileBuilderTemplate template)
    //{
    //    Template = template;
    //    Model = mappingModel.Model;
    //    Mapping = mappingModel.Mapping;
    //    Children = mappingModel.GetChildMappings(template);
    //}

    public virtual IEnumerable<CSharpStatement> GetMappingStatement()
    {
        yield return new CSharpAssignmentStatement(GetToPathText(), GetFromPathText());
    }

    public virtual CSharpStatement GetFromStatement()
    {
        return GetFromPathText();
    }

    public virtual CSharpStatement GetToStatement()
    {
        return GetToPathText();
    }

    protected IList<IElementMappingPathTarget> GetFromPath()
    {
        if (Mapping != null)
        {
            return Mapping.FromPath;
        }

        var mapping = GetAllChildren().First(x => x.Mapping != null).Mapping;
        if (GetAllChildren().Count(x => x.Mapping != null) == 1)
        {
            return mapping.FromPath.Take(mapping.FromPath.Count - 1).ToList();
        }
        var toPath = mapping.FromPath.Take(mapping.FromPath.IndexOf(mapping.FromPath
            .Last(x => GetAllChildren().Where(c => c.Mapping != null).All(c => c.Mapping.FromPath.Contains(x)))) + 1)
            .ToList();
        return toPath;
    }

    protected IList<IElementMappingPathTarget> GetToPath()
    {
        if (Mapping != null)
        {
            return Mapping.ToPath;
        }
        var mapping = GetAllChildren().First(x => x.Mapping != null).Mapping;
        var toPath = mapping.ToPath.Take(mapping.ToPath.IndexOf(mapping.ToPath.Single(x => x.Element == Model)) + 1).ToList();
        return toPath;
    }

    private IEnumerable<ICSharpMapping> GetAllChildren()
    {
        return Children.Concat(Children.SelectMany(x => ((CSharpMappingBase)x).GetAllChildren()).ToList());
    }

    public void SetFromReplacement(ICanBeReferencedType type, string replacement)
    {
        if (_fromReplacements.ContainsKey(type))
        {
            _fromReplacements.Remove(type);
        }
        _fromReplacements.Add(type, replacement);
        foreach (var child in Children)
        {
            child.SetFromReplacement(type, replacement);
        }
    }

    public void SetToReplacement(ICanBeReferencedType type, string replacement)
    {
        if (_toReplacements.ContainsKey(type))
        {
            _toReplacements.Remove(type);
        }
        _toReplacements.Add(type, replacement);
        foreach (var child in Children)
        {
            child.SetToReplacement(type, replacement);
        }
    }

    protected string GetFromPathText()
    {
        return GetPathText(GetFromPath(), _fromReplacements);
    }

    protected string GetToPathText()
    {
        return GetPathText(GetToPath(), _toReplacements);
    }

    protected string GetPathText(IList<IElementMappingPathTarget> mappingPath, IDictionary<ICanBeReferencedType, string> replacements)
    {
        var result = "";
        foreach (var mappingPathTarget in mappingPath)
        {
            if (replacements.ContainsKey(mappingPathTarget.Element))
            {
                result = replacements[mappingPathTarget.Element] ?? ""; // if map to null, ignore
            }
            else
            {
                var referenceName = Template.CSharpFile.GetReferenceForModel(mappingPathTarget.Id)?.Name ?? mappingPathTarget.Element.Name;
                result += $"{(result.Length > 0 ? "." : "")}{referenceName}";
                if (mappingPathTarget.Element.TypeReference?.IsNullable == true && mappingPath.Last() != mappingPathTarget)
                {
                    result += "?";
                }
            }
        }
        return result;
    }
}