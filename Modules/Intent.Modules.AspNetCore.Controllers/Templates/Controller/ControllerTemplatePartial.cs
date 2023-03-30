using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Controllers.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class ControllerTemplate : CSharpTemplateBase<ServiceModel, ControllerDecorator>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.AspNetCore.Controllers.Controller";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ControllerTemplate(IOutputTarget outputTarget, ServiceModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource("Domain.Enum");
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.AspNetCore.Authorization")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("Microsoft.AspNetCore.Mvc")
                .AddClass($"{Model.Name.RemoveSuffix("Controller", "Service")}Controller", @class =>
                {
                    @class.AddAttribute("[ApiController]");
                    @class.WithBaseType("ControllerBase");
                    @class.AddConstructor();
                    foreach (var attribute in GetControllerAttributes())
                    {
                        @class.AddAttribute(attribute);
                    }
                    foreach (var operation in Model.Operations.Where(p => p.HasHttpSettings()))
                    {
                        @class.AddMethod($"Task<{GetReturnType(operation)}>", operation.Name.ToPascalCase(), method =>
                        {
                            method.AddMetadata("model", operation);
                            method.Async();
                            method.WithComments(GetOperationComments(operation));
                            foreach (var attribute in GetOperationAttributes(operation))
                            {
                                method.AddAttribute(attribute);
                            }
                            foreach (var parameter in operation.Parameters)
                            {
                                method.AddParameter(GetTypeName(parameter), parameter.Name.ToCamelCase(), param =>
                                {
                                    var attr = GetParameterBindingAttribute(operation, parameter);
                                    if (!string.IsNullOrWhiteSpace(attr))
                                    {
                                        param.AddAttribute(attr);
                                    }
                                });
                            }

                            method.AddParameter("CancellationToken", "cancellationToken");
                        });
                    }
                });
        }
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name.RemoveSuffix("Controller", "Service")}Controller",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: this.GetFolderPath());
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public HttpVerb GetHttpVerb(OperationModel operation)
        {
            var verb = operation.GetHttpSettings().Verb();

            return Enum.TryParse(verb.Value, ignoreCase: true, out HttpVerb verbEnum) ? verbEnum : HttpVerb.Post;
        }

        private IEnumerable<string> GetControllerAttributes()
        {
            var attributes = new List<string>();
            if (IsControllerSecured())
            {
                // We can extend this later (if desired) to have multiple Secure stereotypes create
                // multiple Authorization Models.
                var authModel = new AuthorizationModel();
                GetDecorators().ToList().ForEach(x => x.UpdateServiceAuthorization(authModel, new ServiceSecureModel(Model, Model.GetSecured())));
                attributes.Add(GetAuthorizationAttribute(authModel));
            }
            else if (Model.HasUnsecured())
            {
                attributes.Add("[AllowAnonymous]");
            }

            attributes.Add(
                $@"[Route(""{(string.IsNullOrWhiteSpace(Model.GetHttpServiceSettings().Route()) ? "api/[controller]" : Model.GetHttpServiceSettings().Route())}"")]");
            attributes.AddRange(GetDecorators().SelectMany(x => x.GetControllerAttributes()));
            return attributes;
        }

        private IEnumerable<string> GetOperationComments(OperationModel operation)
        {
            var lines = new List<string>
            {
                "/// <summary>"
            };

            if (!string.IsNullOrWhiteSpace(operation.Comment))
            {
                lines.AddRange(operation.Comment
                    .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(commentLine => $"/// {commentLine}"));
            }

            lines.Add("/// </summary>");
            switch (GetHttpVerb(operation))
            {
                case HttpVerb.Get:
                    lines.Add(
                        $"/// <response code=\"200\">Returns the specified {GetTypeName(operation.ReturnType).Replace("<", "&lt;").Replace(">", "&gt;")}.</response>");
                    break;
                case HttpVerb.Post:
                    lines.Add("/// <response code=\"201\">Successfully created.</response>");
                    break;
                case HttpVerb.Patch:
                case HttpVerb.Put:
                    lines.Add($"/// <response code=\"{(operation.ReturnType != null ? "200" : "204")}\">Successfully updated.</response>");
                    break;
                case HttpVerb.Delete:
                    lines.Add("/// <response code=\"200\">Successfully deleted.</response>");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (operation.Parameters.Any())
            {
                lines.Add("/// <response code=\"400\">One or more validation errors have occurred.</response>");
            }

            if (IsOperationSecured(operation))
            {
                lines.Add("/// <response code=\"401\">Unauthorized request.</response>");
                lines.Add("/// <response code=\"403\">Forbidden request.</response>");
            }

            if (GetHttpVerb(operation) == HttpVerb.Get && operation.ReturnType?.IsCollection == false)
            {
                lines.Add(
                    $"/// <response code=\"404\">Can't find an {GetTypeName(operation.ReturnType).Replace("<", "&lt;").Replace(">", "&gt;")} with the parameters provided.</response>");
            }

            return lines;
        }

        private IEnumerable<string> GetOperationAttributes(OperationModel operation)
        {
            var attributes = new List<string>();
            attributes.AddRange(GetDecorators().SelectMany(x => x.GetOperationAttributes(operation)));
            attributes.Add(GetHttpVerbAndPath(operation));
            if (operation.HasSecured() || operation.HasUnsecured())
            {
                if ((!IsControllerSecured() && IsOperationSecured(operation)) ||
                    !string.IsNullOrWhiteSpace(operation.GetSecured()?.Roles()))
                {
                    // We can extend this later (if desired) to have multiple Secure stereotypes create
                    // multiple Authorization Models.
                    // NOTE: GCB - the following is an anti-pattern imo @Dandre. Passing in an object to some method which results in mutations is
                    // bad programming practice. It forces us to break encapsulation to determine what is in fact going on. I will replace this with a 
                    // better approach at a later stage.
                    // TODO: GCB - convert the auth system to use a generic role/policy based system that decouples it from WebApi module.
                    var authModel = new AuthorizationModel();
                    GetDecorators().ToList().ForEach(x =>
                        x.UpdateOperationAuthorization(authModel,
                            new OperationSecureModel(operation, operation.GetSecured())));
                    attributes.Add(GetAuthorizationAttribute(authModel));
                }
                else if (IsControllerSecured() &&
                         !IsOperationSecured(operation) &&
                         operation.HasUnsecured())
                {
                    attributes.Add("[AllowAnonymous]");
                }
            }

            var apiResponse = operation.ReturnType != null ? $"typeof({GetTypeName(operation)}), " : string.Empty;
            if (operation.GetHttpSettings().ReturnTypeMediatype().IsApplicationJson() &&
				operation.ReturnType != null &&
                (GetTypeInfo(operation.ReturnType).IsPrimitive || operation.ReturnType.HasStringType()))
            {
                apiResponse = $"typeof({this.GetJsonResponseName()}<{GetTypeName(operation)}>), ";
            }

            switch (GetHttpVerb(operation))
            {
                case HttpVerb.Get:
                    attributes.Add($@"[ProducesResponseType({apiResponse}StatusCodes.Status200OK)]");
                    break;
                case HttpVerb.Post:
                    attributes.Add($@"[ProducesResponseType({apiResponse}StatusCodes.Status201Created)]");
                    break;
                case HttpVerb.Put:
                case HttpVerb.Patch:
                    attributes.Add(operation.ReturnType != null
                        ? $@"[ProducesResponseType({apiResponse}StatusCodes.Status200OK)]"
                        : @"[ProducesResponseType(StatusCodes.Status204NoContent)]");
                    break;
                case HttpVerb.Delete:
                    attributes.Add($@"[ProducesResponseType({apiResponse}StatusCodes.Status200OK)]");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (operation.Parameters.Any())
            {
                attributes.Add(@"[ProducesResponseType(StatusCodes.Status400BadRequest)]");
            }

            if (IsOperationSecured(operation))
            {
                attributes.Add(@"[ProducesResponseType(StatusCodes.Status401Unauthorized)]");
                attributes.Add(@"[ProducesResponseType(StatusCodes.Status403Forbidden)]");
            }

            if (GetHttpVerb(operation) == HttpVerb.Get && operation.ReturnType?.IsCollection == false)
            {
                attributes.Add(@"[ProducesResponseType(StatusCodes.Status404NotFound)]");
            }

            attributes.Add(@"[ProducesResponseType(StatusCodes.Status500InternalServerError)]");
            return attributes;
        }

        private static string GetAuthorizationAttribute(AuthorizationModel authorizationModel)
        {
            if (authorizationModel == null)
            {
                throw new ArgumentNullException(nameof(authorizationModel));
            }

            var fieldExpressions = new List<string>();

            if (!string.IsNullOrWhiteSpace(authorizationModel.RolesExpression))
            {
                fieldExpressions.Add($"Roles = {authorizationModel.RolesExpression}");
            }

            if (!string.IsNullOrWhiteSpace(authorizationModel.Policy))
            {
                fieldExpressions.Add($"Policy = {authorizationModel.Policy}");
            }

            if (!string.IsNullOrWhiteSpace(authorizationModel.AuthenticationSchemesExpression))
            {
                fieldExpressions.Add($"AuthenticationSchemes = {authorizationModel.AuthenticationSchemesExpression}");
            }

            if (fieldExpressions.Any())
            {
                return $"[Authorize ({string.Join(", ", fieldExpressions)})]";
            }

            return "[Authorize]";
        }

        private bool IsControllerSecured()
        {
            return ExecutionContext.Settings.GetAPISettings().DefaultAPISecurity().AsEnum() switch
            {
                APISettings.DefaultAPISecurityOptionsEnum.Secured => Model.HasSecured() || !Model.HasUnsecured(),
                APISettings.DefaultAPISecurityOptionsEnum.Unsecured => Model.HasSecured(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool IsOperationSecured(OperationModel operation)
        {
            if (!operation.HasSecured() && !operation.HasUnsecured())
            {
                return IsControllerSecured();
            }

            return IsControllerSecured()
                ? operation.HasSecured() || !operation.HasUnsecured()
                : operation.HasSecured();
        }

        private string GetHttpVerbAndPath(OperationModel o)
        {
            return
                $"[Http{GetHttpVerb(o).ToString().ToLower().ToPascalCase()}{(GetPath(o) != null ? $"(\"{GetPath(o)}\")" : "")}]";
        }

        private string GetReturnType(OperationModel operation)
        {
            return operation.ReturnType == null
                ? "ActionResult"
                : $"ActionResult<{GetTypeName(operation.TypeReference)}>";
        }

        private static string GetPath(OperationModel operation)
        {
            var path = operation.GetHttpSettings().Route();
            return !string.IsNullOrWhiteSpace(path) ? path : null;
        }

        private static string GetParameterBindingAttribute(OperationModel operation, ParameterModel parameter)
        {
            if (parameter.GetParameterSettings().Source().IsDefault())
            {
                if ((operation.GetHttpSettings().Verb().IsGET() || operation.GetHttpSettings().Verb().IsDELETE()) &&
                    !parameter.TypeReference.Element.IsTypeDefinitionModel())
                {
                    return "[FromQuery]";
                }

                if ((operation.GetHttpSettings().Verb().IsPOST() || operation.GetHttpSettings().Verb().IsPUT()) &&
                    !parameter.TypeReference.Element.IsTypeDefinitionModel())
                {
                    return "[FromBody]";
                }

                if (parameter.TypeReference.Element.IsTypeDefinitionModel() &&
                    operation.GetHttpSettings().Route()?.Contains($"{{{parameter.Name}}}") == true)
                {
                    return "[FromRoute]";
                }

                return string.Empty;
            }

            return parameter.GetParameterSettings().Source().AsEnum() switch
            {
                ParameterModelStereotypeExtensions.ParameterSettings.SourceOptionsEnum.FromBody => "[FromBody]",
                ParameterModelStereotypeExtensions.ParameterSettings.SourceOptionsEnum.FromForm => "[FromForm]",
                ParameterModelStereotypeExtensions.ParameterSettings.SourceOptionsEnum.FromHeader =>
                    $@"[FromHeader(Name = ""{parameter.GetParameterSettings().HeaderName()}"")]",
                ParameterModelStereotypeExtensions.ParameterSettings.SourceOptionsEnum.FromQuery => "[FromQuery]",
                ParameterModelStereotypeExtensions.ParameterSettings.SourceOptionsEnum.FromRoute => "[FromRoute]",
                _ => string.Empty
            };
        }

        public enum HttpVerb
        {
            Get,
            Post,
            Put,
            Patch,
            Delete,

            // ReSharper disable InconsistentNaming
            /// <summary>
            /// Obsolete. Use <see cref="Get"/> instead.
            /// </summary>
            [Obsolete(WillBeRemovedIn.Version4)] GET = 0,

            /// <summary>
            /// Obsolete. Use <see cref="Post"/> instead.
            /// </summary>
            [Obsolete(WillBeRemovedIn.Version4)] POST = 1,

            /// <summary>
            /// Obsolete. Use <see cref="Put"/> instead.
            /// </summary>
            [Obsolete(WillBeRemovedIn.Version4)] PUT = 2,

            /// <summary>
            /// Obsolete. Use <see cref="Patch"/> instead.
            /// </summary>
            [Obsolete(WillBeRemovedIn.Version4)] PATCH = 3,

            /// <summary>
            /// Obsolete. Use <see cref="Delete"/> instead.
            /// </summary>
            [Obsolete(WillBeRemovedIn.Version4)] DELETE = 4
            // ReSharper restore InconsistentNaming
        }

    }
}