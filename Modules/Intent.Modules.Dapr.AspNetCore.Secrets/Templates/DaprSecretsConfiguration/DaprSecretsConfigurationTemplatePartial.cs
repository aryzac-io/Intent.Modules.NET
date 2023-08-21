using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Secrets.Templates.DaprSecretsConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DaprSecretsConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.Secrets.DaprSecretsConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DaprSecretsConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Collections.Generic")
                .AddUsing("Dapr.Client")
                .AddUsing("Dapr.Extensions.Configuration")
                .AddUsing("Microsoft.AspNetCore.Builder")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddClass($"DaprSecretsConfiguration", @class =>
                {
                    this.ApplyAppSetting("Dapr.Secrets:StoreName", "secret-store");
                    @class.Static();
                    @class.AddMethod("void", "AddDaprSecretStore", method =>
                    {

                        method.Static()
                            .AddParameter("IApplicationBuilder", "app", p => p.WithThisModifier())
                            .AddParameter("IConfiguration", "configuration")
                            .AddStatement("string store = configuration.GetValue<string>(\"Dapr.Secrets:StoreName\") ?? \"secret-store\" ;")
                            .AddStatement("string? descriptorsList = configuration.GetValue<string?>(\"Dapr.Secrets:Descriptors\");")
                            .AddStatement("var secretDescriptors = CreateDescriptors(descriptorsList);")
                            .AddStatement("var client = new DaprClientBuilder().Build();")
                            .AddStatement("var daprSecretsLoader = new DaprSecretStoreConfigurationSourceCopy();")
                            .AddStatement("var data = daprSecretsLoader.Load(client, store, secretDescriptors);")
                            .AddForEachStatement("kvp", "data", loop =>
                            {
                                loop.AddStatement("configuration[kvp.Key] = kvp.Value;");
                            })
                            ;
                    });
                    @class.AddMethod("List<DaprSecretDescriptor>?", "CreateDescriptors", method =>
                    {

                        method.Static()
                            .AddParameter("string?", "descriptorsList")
                            .AddIfStatement("string.IsNullOrWhiteSpace(descriptorsList)", body => { body.AddStatement("return null;"); })
                            .AddStatement("var result = new List<DaprSecretDescriptor>();")
                            .AddStatement("string[] descriptors = descriptorsList.Trim().Split(',');")
                            .AddForEachStatement("descriptor", "descriptors", loop =>
                            {
                                loop.AddStatement("result.Add(new DaprSecretDescriptor(descriptor.Trim()));");
                            })
                            .AddStatement("return result;")
                            ;
                    });
                    @class.AddNestedClass("DaprSecretStoreConfigurationSourceCopy", child =>
                    {
                        child.Private()
                            .WithComments(@"
/// <summary>
/// This class is basically a copy of DaprSecretStoreConfigurationSource in the Dapr.Extensions.Configuration assembly.
/// A standard Dapr implementation would load configuration in CreateHostBuilder (Program.cs)
/// because We are using SideKick we can not load the Secrets Config until the SideCar is ready which happened after ServicesConfiguration (StartUp.cs).
/// </summary>");
                        child.AddField("TimeSpan", "_sidecarWaitTimeout", f => f.PrivateReadOnly().WithAssignment("TimeSpan.FromSeconds(35)"));
                        child.AddField("bool", "_normalizeKey", f => f.PrivateReadOnly().WithAssignment("true"));
                        child.AddField("IList<string>", "_keyDelimiters", f => f.PrivateReadOnly().WithAssignment("new List<string> { \"__\" }"));
                        child.AddField("IReadOnlyDictionary<string, string>?", "_metadata", f => f.PrivateReadOnly().WithAssignment("null"));
                        child.AddConstructor();
                        child.AddMethod("string", "NormalizeKey", method =>
                        {
                            method
                            .Private()
                            .Static()
                            .AddParameter("IList<string>", "keyDelimiters")
                            .AddParameter("string", "key")
                            .AddIfStatement("keyDelimiters?.Count > 0", condition =>
                            {
                                condition.AddForEachStatement("keyDelimiter", "keyDelimiters", stmt =>
                                {
                                    stmt.AddStatement("key = key.Replace(keyDelimiter, ConfigurationPath.KeyDelimiter);");
                                });
                            })
                            .AddStatement("return key;")
                            ;
                        });
                        child.AddMethod("Dictionary<string, string>", "Load", method =>
                        {
                            method.AddParameter("DaprClient", "client");
                            method.AddParameter("string", "store");
                            method.AddParameter("List<DaprSecretDescriptor>?", "secretDescriptors", p => p.WithDefaultValue("null"));
                            method.AddStatement(@"var data = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

                // Wait for the Dapr Sidecar to report healthy before attempting to fetch secrets.
                using (var tokenSource = new CancellationTokenSource(_sidecarWaitTimeout))
                {
                    client.WaitForSidecarAsync(tokenSource.Token).GetAwaiter().GetResult();
                }

                if (secretDescriptors != null)
                {
                    foreach (var secretDescriptor in secretDescriptors)
                    {
                        var result = client.GetSecretAsync(store, secretDescriptor.SecretName, secretDescriptor.Metadata).GetAwaiter().GetResult();

                        foreach (var key in result.Keys)
                        {
                            if (data.ContainsKey(key))
                            {
                                throw new InvalidOperationException($""A duplicate key '{key}' was found in the secret store '{store}'. Please remove any duplicates from your secret store."");
                            }

                            data.Add(_normalizeKey ? NormalizeKey(_keyDelimiters, key) : key, result[key]);
                        }
                    }
                }
                else
                {
                    var result = client.GetBulkSecretAsync(store, _metadata).GetAwaiter().GetResult();
                    foreach (var key in result.Keys)
                    {
                        foreach (var secret in result[key])
                        {
                            if (data.ContainsKey(secret.Key))
                            {
                                throw new InvalidOperationException($""A duplicate key '{secret.Key}' was found in the secret store '{store}'. Please remove any duplicates from your secret store."");
                            }

                            data.Add(_normalizeKey ? NormalizeKey(_keyDelimiters, secret.Key) : secret.Key, secret.Value);
                        }
                    }
                }
                return data;
");
                        });
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}