using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller.Models;

public class ServiceControllerModel : IControllerModel
{
    private readonly ServiceModel _model;

    public ServiceControllerModel(ServiceModel model)
    {
        if (model.HasSecured() && model.HasUnsecured())
        {
            throw new Exception($"Controller {model.Name} cannot require authorization and allow-anonymous at the same time");
        }

        _model = model;
        RequiresAuthorization = model.HasSecured();
        AllowAnonymous = model.HasUnsecured();
        Route = GetControllerRoute(model.GetHttpServiceSettings()?.Route());
        AuthorizationModel = GetAuthorizationModel(model.GetSecured()?.Roles());
        Operations = model.Operations
            .Where(x => x.HasHttpSettings())
            .Select(GetOperation)
            .ToList();
        ApplicableVersions = model.GetApiVersionSettings()
            ?.ApplicableVersions()
            .Select(s => new ControllerApiVersionModel(s))
            .Cast<IApiVersionModel>()
            .ToList() ?? new List<IApiVersionModel>();
    }

    private static AuthorizationModel GetAuthorizationModel(string roles)
    {
        return new AuthorizationModel
        {
            RolesExpression = !string.IsNullOrWhiteSpace(roles)
                ? @$"{string.Join("+", roles.Split('+', StringSplitOptions.RemoveEmptyEntries)
                    .Select(group => string.Join(",", group.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim()))))}"
                : null
        };
    }

    private string GetControllerRoute(string route)
    {
        var serviceName = _model.Name.RemoveSuffix("Controller", "Service");

        route ??= "api/[controller]";
        var segments = route
            .Split('/')
            .Select(segment => segment.Equals(serviceName, StringComparison.OrdinalIgnoreCase) ? "[controller]" : segment);

        return string.Join('/', segments);
    }

    private IControllerOperationModel GetOperation(OperationModel model)
    {
        var httpEndpoint = HttpEndpointModelFactory.GetEndpoint(model.InternalElement)!;

        return new ControllerOperationModel(
            name: httpEndpoint.Name,
            element: model.InternalElement,
            verb: httpEndpoint.Verb,
            route: httpEndpoint.SubRoute,
            mediaType: httpEndpoint.MediaType,
            requiresAuthorization: httpEndpoint.RequiresAuthorization,
            allowAnonymous: httpEndpoint.AllowAnonymous,
            authorizationModel: GetAuthorizationModel(model.GetSecured()?.Roles()),
            parameters: httpEndpoint.Inputs.Select(GetInput).ToList(),
            applicableVersions: model.GetApiVersionSettings()
                ?.ApplicableVersions()
                .Select(s => new ControllerApiVersionModel(s))
                .Cast<IApiVersionModel>()
                .ToList() ?? new List<IApiVersionModel>());
    }

    private static IControllerParameterModel GetInput(IHttpEndpointInputModel model)
    {
        return new ControllerParameterModel(
            id: model.Id,
            name: model.Name,
            typeReference: model.TypeReference,
            source: model.Source,
            headerName: model.HeaderName,
            queryStringName: model.QueryStringName,
            mappedPayloadProperty: model.MappedPayloadProperty,
            value: model.Value);
    }

    public string Id => _model.Id;
    public FolderModel Folder => _model.Folder;
    public string Name => _model.Name;
    public bool RequiresAuthorization { get; }
    public bool AllowAnonymous { get; }
    public IAuthorizationModel AuthorizationModel { get; }
    public string Comment => _model.Comment;
    public string Route { get; }
    public IList<IControllerOperationModel> Operations { get; }
    public IList<IApiVersionModel> ApplicableVersions { get; }
}


public class ControllerOperationModel : IControllerOperationModel
{
    public ControllerOperationModel(
        string name,
        IElement element,
        HttpVerb verb,
        string route,
        HttpMediaType? mediaType,
        bool requiresAuthorization,
        bool allowAnonymous,
        IAuthorizationModel authorizationModel,
        IList<IControllerParameterModel> parameters, 
        IList<IApiVersionModel> applicableVersions)
    {
        Id = element.Id;
        Name = name;
        TypeReference = element.TypeReference;
        Comment = element.Comment;
        InternalElement = element;
        Verb = verb;
        Route = route;
        MediaType = mediaType;
        RequiresAuthorization = requiresAuthorization;
        AllowAnonymous = allowAnonymous;
        AuthorizationModel = authorizationModel;
        Parameters = parameters;
        ApplicableVersions = applicableVersions;
    }

    public string Id { get; }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public string Comment { get; }
    public IElement InternalElement { get; }
    public ITypeReference ReturnType => TypeReference.Element != null ? TypeReference : null;
    public HttpVerb Verb { get; }
    public string Route { get; }
    public HttpMediaType? MediaType { get; }
    public bool RequiresAuthorization { get; }
    public bool AllowAnonymous { get; }
    public IAuthorizationModel AuthorizationModel { get; }
    public IList<IControllerParameterModel> Parameters { get; }
    public IList<IApiVersionModel> ApplicableVersions { get; }
}

public class ControllerParameterModel : IControllerParameterModel
{
    public ControllerParameterModel(
        string id,
        string name,
        ITypeReference typeReference,
        HttpInputSource? source,
        string headerName,
        string queryStringName,
        ICanBeReferencedType mappedPayloadProperty,
        string value)
    {
        Id = id;
        Name = name;
        TypeReference = typeReference;
        Source = source;
        HeaderName = headerName;
        QueryStringName = queryStringName;
        MappedPayloadProperty = mappedPayloadProperty;
        Value = value;
    }

    public string Id { get; }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public HttpInputSource? Source { get; }
    public string HeaderName { get; }
    public string QueryStringName { get; }
    public ICanBeReferencedType MappedPayloadProperty { get; }
    public string Value { get; }
}

public class ControllerApiVersionModel : IApiVersionModel
{
    public ControllerApiVersionModel(string definitionName, string version, bool isDeprecated)
    {
        DefinitionName = definitionName;
        Version = version;
        IsDeprecated = isDeprecated;
    }

    public ControllerApiVersionModel(ICanBeReferencedType element)
    {
        var versionModel = element.AsVersionModel();
        if (versionModel == null)
        {
            throw new InvalidOperationException($"Element {element.Id} [{element.Name}] is not a VersionModel.");
        }

        DefinitionName = versionModel.ApiVersion?.Name;
        Version = versionModel.Name;
        IsDeprecated = versionModel.GetVersionSettings()?.IsDeprecated() == true;
    }

    public string DefinitionName { get; }
    public string Version { get; }
    public bool IsDeprecated { get; }

    protected bool Equals(ControllerApiVersionModel other)
    {
        return DefinitionName == other.DefinitionName && 
               Version == other.Version && 
               IsDeprecated == other.IsDeprecated;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ControllerApiVersionModel)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DefinitionName, Version, IsDeprecated);
    }
}