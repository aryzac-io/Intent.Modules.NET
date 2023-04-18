using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients;
using Intent.Modules.Application.Contracts.Clients.Templates;
using Intent.Modules.Application.Contracts.Clients.Templates.DtoContract;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;
using ParameterModel = Intent.Modelers.Services.Api.ParameterModel;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClient;

public class HttpClientGenerator
{
    private readonly CSharpTemplateBase<ServiceProxyModel> _template;
    private readonly string _httpClientRequestExceptionTemplateId;
    private readonly string _jsonResponseTemplateId;
    private readonly MappedModel _mappedModel;

    private HttpClientGenerator(
        CSharpTemplateBase<ServiceProxyModel> template,
        string httpClientRequestExceptionTemplateId,
        string jsonResponseTemplateId)
    {
        _template = template;
        _httpClientRequestExceptionTemplateId = httpClientRequestExceptionTemplateId;
        _jsonResponseTemplateId = jsonResponseTemplateId;
    }

    public static CSharpFile CreateCSharpFile(
        CSharpTemplateBase<ServiceProxyModel> template,
        string httpClientRequestExceptionTemplateId,
        string jsonResponseTemplateId)
    {
        return new HttpClientGenerator(template, httpClientRequestExceptionTemplateId, jsonResponseTemplateId).Create();
    }

    private CSharpFile Create()
    {
        ServiceMetadataQueries.Validate(_template, _template.Model);

        _template.AddNugetDependency(NuGetPackages.MicrosoftExtensionsHttp);
        _template.AddNugetDependency(NuGetPackages.MicrosoftAspNetCoreWebUtilities);

        _template.AddTypeSource(ServiceContractTemplate.TemplateId);
        _template.AddTypeSource(DtoContractTemplate.TemplateId).WithCollectionFormat("List<{0}>");
        _template.SetDefaultCollectionFormatter(CSharpCollectionFormatter.Create("List<{0}>"));

        return new CSharpFile(_template.GetNamespace(), _template.GetFolderPath())
            .AddUsing("System")
            .AddUsing("System.Collections.Generic")
            .AddUsing("System.IO")
            .AddUsing("System.Linq")
            .AddUsing("System.Net")
            .AddUsing("System.Net.Http")
            .AddUsing("System.Net.Http.Headers")
            .AddUsing("System.Text")
            .AddUsing("System.Text.Json")
            .AddUsing("System.Threading")
            .AddUsing("System.Threading.Tasks")
            .AddUsing("Microsoft.AspNetCore.WebUtilities")
            .IntentManagedFully()
            .AddClass($"{_template.Model.Name.RemoveSuffix("Client")}HttpClient", @class =>
            {
                @class
                    .ImplementsInterface(_template.GetServiceContractName())
                    .AddField("JsonSerializerOptions", "_serializerOptions", f => f.PrivateReadOnly())
                    .AddConstructor(constructor => constructor
                        .AddParameter("HttpClient", "httpClient", p => p.IntroduceReadonlyField())
                        .AddStatement(@"_serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };"));

                foreach (var operation in _mappedModel.Operations)
                {
                    @class.AddMethod(GetReturnType(operation), GetOperationName(operation), method =>
                    {
                        method.Async();

                        foreach (var parameter in operation.Parameters)
                        {
                            method.AddParameter(_template.GetTypeName(parameter.Type), parameter.Name.ToParameterName());
                        }

                        method.AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"));

                        // We're leveraging the C# $"" notation to actually take leverage of the parameters
                        // that are meant to be Route-based.
                        method.AddStatement($"var relativeUri = $\"{operation.RelativeUri}\";");

                        if (HasQueryParameter(operation))
                        {
                            method.AddStatement("var queryParams = new Dictionary<string, string>();", s => s.SeparatedFromPrevious());

                            foreach (var queryParameter in GetQueryParameters(operation))
                            {
                                method.AddStatement($"queryParams.Add(\"{queryParameter.Name.ToCamelCase()}\", {GetParameterValueExpression(queryParameter)});");
                            }

                            method.AddStatement("relativeUri = QueryHelpers.AddQueryString(relativeUri, queryParams);");
                        }

                        method.AddStatement($"var request = new HttpRequestMessage({GetHttpVerb(operation)}, relativeUri);");
                        method.AddStatement("request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(\"application/json\"));");

                        foreach (var headerParameter in GetHeaderParameters(operation))
                        {
                            method.AddStatement($"request.Headers.Add(\"{headerParameter.HeaderName}\", {headerParameter.Parameter.Name.ToParameterName()});");
                        }

                        if (HasBodyParameter(operation))
                        {
                            method.AddStatement($"var content = JsonSerializer.Serialize({GetBodyParameterName(operation)}, _serializerOptions);", s => s.SeparatedFromPrevious());
                            method.AddStatement("request.Content = new StringContent(content, Encoding.Default, \"application/json\");");
                        }
                        else if (HasFormUrlEncodedParameter(operation))
                        {
                            method.AddStatement("var formVariables = new List<KeyValuePair<string, string>>();", s => s.SeparatedFromPrevious());

                            foreach (var formParameter in GetFormUrlEncodedParameters(operation))
                            {
                                method.AddStatement($"formVariables.Add(new KeyValuePair<string, string>(\"{formParameter.Name.ToPascalCase()}\", {GetParameterValueExpression(formParameter)}));");
                            }

                            method.AddStatement("var content = new FormUrlEncodedContent(formVariables);");
                            method.AddStatement("request.Content = content;");
                        }

                        method.AddStatementBlock("using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))", usingResponseBlock =>
                        {
                            usingResponseBlock.SeparatedFromPrevious();

                            usingResponseBlock.AddStatementBlock("if (!response.IsSuccessStatusCode)", s => s
                                .AddStatement($"throw await {_template.GetTypeName(_httpClientRequestExceptionTemplateId)}.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);")
                            );

                            if (HasResponseType(operation))
                            {
                                usingResponseBlock.AddStatementBlock("if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)", s => s
                                    .AddStatement("return default;")
                                );

                                usingResponseBlock.AddStatementBlock("using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))", usingContentStreamBlock =>
                                {
                                    usingContentStreamBlock.SeparatedFromPrevious();

                                    if (HasWrappedReturnType(operation) && (IsReturnTypePrimitive(operation) || operation.ReturnType.HasStringType()))
                                    {
                                        usingContentStreamBlock.AddStatement($"var wrappedObj = await JsonSerializer.DeserializeAsync<{_template.GetTypeName(_jsonResponseTemplateId)}<{_template.GetTypeName(operation.ReturnType)}>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);");
                                        usingContentStreamBlock.AddStatement("return wrappedObj.Value;");
                                    }
                                    else if (!HasWrappedReturnType(operation) && operation.ReturnType.HasStringType() && !operation.ReturnType.IsCollection)
                                    {
                                        usingContentStreamBlock.AddStatement("var str = await new StreamReader(contentStream).ReadToEndAsync().ConfigureAwait(false);");
                                        usingContentStreamBlock.AddStatement("if (str != null && (str.StartsWith(@\"\"\"\") || str.StartsWith(\"'\"))) { str = str.Substring(1, str.Length - 2); }");
                                        usingContentStreamBlock.AddStatement("return str;");
                                    }
                                    else if (!HasWrappedReturnType(operation) && IsReturnTypePrimitive(operation))
                                    {
                                        usingContentStreamBlock.AddStatement("var str = await new StreamReader(contentStream).ReadToEndAsync().ConfigureAwait(false);");
                                        usingContentStreamBlock.AddStatement("if (str != null && (str.StartsWith(@\"\"\"\") || str.StartsWith(\"'\"))) { str = str.Substring(1, str.Length - 2); };");
                                        usingContentStreamBlock.AddStatement($"return {_template.GetTypeName(operation.ReturnType)}.Parse(str);");
                                    }
                                    else
                                    {
                                        usingContentStreamBlock.AddStatement($"return await JsonSerializer.DeserializeAsync<{_template.GetTypeName(operation.ReturnType)}>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);");
                                    }
                                });
                            }
                        });
                    });
                }

                @class.AddMethod("void", "Dispose");
            });
    }

    private string GetReturnType(MappedOperation operation)
    {
        if (operation.ReturnType == null)
        {
            return "Task";
        }

        return $"Task<{_template.GetTypeName(operation.ReturnType)}>";
    }

    private static string GetOperationName(MappedOperation operation)
    {
        return $"{operation.Name.ToPascalCase().RemoveSuffix("Async")}Async";
    }

    private bool HasQueryParameter(OperationModel operation)
    {
        return ServiceMetadataQueries.GetQueryParameters(_template, operation).Any();
    }

    private IEnumerable<ParameterModel> GetQueryParameters(OperationModel operation)
    {
        return ServiceMetadataQueries.GetQueryParameters(_template, operation);
    }

    private static string GetHttpVerb(OperationModel operation)
    {
        return $"HttpMethod.{ServiceMetadataQueries.GetHttpVerb(operation)}";
    }

    private static IEnumerable<ServiceMetadataQueries.HeaderParameter> GetHeaderParameters(OperationModel operation)
    {
        return ServiceMetadataQueries.GetHeaderParameters(operation);
    }

    private bool HasBodyParameter(OperationModel operation)
    {
        return ServiceMetadataQueries.GetBodyParameter(_template, operation) != null;
    }

    private string GetBodyParameterName(OperationModel operation)
    {
        return ServiceMetadataQueries.GetBodyParameter(_template, operation).Name.ToParameterName();
    }

    private static bool HasFormUrlEncodedParameter(OperationModel operation)
    {
        return ServiceMetadataQueries.GetFormUrlEncodedParameters(operation).Any();
    }

    private static IEnumerable<ParameterModel> GetFormUrlEncodedParameters(OperationModel operation)
    {
        return ServiceMetadataQueries.GetFormUrlEncodedParameters(operation);
    }

    private static bool HasResponseType(OperationModel operation)
    {
        return operation.ReturnType != null;
    }

    private static bool HasWrappedReturnType(OperationModel operationModel)
    {
        return ServiceMetadataQueries.HasJsonWrappedReturnType(operationModel);
    }

    private bool IsReturnTypePrimitive(OperationModel operation)
    {
        return _template.GetTypeInfo(operation.ReturnType).IsPrimitive && !operation.ReturnType.IsCollection;
    }

    private static string GetParameterValueExpression(ParameterModel parameter)
    {
        return !parameter.TypeReference.HasStringType()
            ? $"{parameter.Name.ToParameterName()}.ToString()"
            : parameter.Name.ToParameterName();
    }
}