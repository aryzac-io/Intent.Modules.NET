﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.NuGet;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.Templates;

public abstract class VisualStudioProjectTemplateBase<TModel> : IntentFileTemplateBase<TModel>, IVisualStudioProjectTemplate
    where TModel : IVisualStudioProject
{
    private string _fileContent;
    protected VisualStudioProjectTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
    {
    }

    public string ProjectId => Model.Id;
    public string Name => Model.Name;
    public string FilePath => FileMetadata.GetFilePath();
    IVisualStudioProject IVisualStudioProjectTemplate.Project => Model;

    public string LoadContent()
    {
        var change = ExecutionContext.ChangeManager.FindChange(FilePath);
        if (change != null)
        {
            return change.Content;
        }

        if (_fileContent == null)
        {
            TryGetExistingFileContent(out _fileContent);
        }

        return _fileContent;
    }

    public void UpdateContent(string content, ISoftwareFactoryEventDispatcher sfEventDispatcher)
    {
        // Normalize the content of both by parsing with no whitespace and calling .ToString()
        var targetContent = XDocument.Parse(content).ToString();
        var existingContent = LoadContent();

        if (existingContent == targetContent)
        {
            return;
        }

        var change = ExecutionContext.ChangeManager.FindChange(FilePath);
        if (change != null)
        {
            change.ChangeContent(content);
            return;
        }

        sfEventDispatcher.Publish(new SoftwareFactoryEvent(SoftwareFactoryEvents.OverwriteFileCommand, new Dictionary<string, string>
        {
            ["FullFileName"] = FilePath,
            ["Context"] = ToString(),
            ["Content"] = content
        }));
    }

    public virtual IEnumerable<INugetPackageInfo> RequestedNugetPackages() => OutputTarget.NugetPackages();

    public IEnumerable<string> GetTargetFrameworks() => Model.TargetFrameworkVersion();

    public override void OnCreated()
    {
        base.OnCreated();
        ExecutionContext.EventDispatcher.Publish(new VisualStudioProjectCreatedEvent(this));
    }

    /// <summary>
    /// This method has been <see langword="sealed"/> to enforce using existing content if it
    /// exists as well as doing a semantic comparison of the result of the xml to avoid
    /// whitespace only changes from occurring. Use <see cref="ApplyAdditionalTransforms"/>
    /// to make changes to the content that was either already existing or generated for the
    /// first time by the <see cref="TransformText"/> method.
    /// </summary>
    public sealed override string RunTemplate()
    {
        var hadExistingContent = TryGetExistingFileContent(out var existingFileContent);

        var content = hadExistingContent
            ? existingFileContent
            : base.RunTemplate();

        content = ApplyAdditionalTransforms(content);

        var doc = XDocument.Parse(content);

        var hasChange = ApplySettings(doc);

        return !hadExistingContent || (hasChange && !XmlHelper.IsSemanticallyTheSame(existingFileContent, doc))
            ? doc.ToFormattedProjectString()
            : existingFileContent;
    }

    /// <summary>
    /// Used to return the initial template content if there is no existing file.
    /// </summary>
    /// <remarks>
    /// Do not put any additional logic in your implementation as this method is only called
    /// when there is no existing file, instead do so in <see cref="ApplyAdditionalTransforms"/>
    /// and <see cref="RunTemplate"/> will ensure that it is passed the existing file content if
    /// it exists or otherwise the result of this <see cref="TransformText"/> method if the file
    /// is being generated for the first time.
    /// </remarks>
    public abstract override string TransformText();

    /// <summary>
    /// Override this method if there are additional changes to the output that you wish to perform.
    /// </summary>
    /// <remarks>
    /// This method is called by <see cref="RunTemplate"/> with either the existing file content
    /// if it exists or otherwise the result of <see cref="TransformText"/> being used for the
    /// <paramref name="existingFileOrTransformTextContent"/>.
    /// </remarks>
    protected virtual string ApplyAdditionalTransforms(string existingFileOrTransformTextContent) => existingFileOrTransformTextContent;

    public override ITemplateFileConfig GetTemplateFileConfig()
    {
        return new TemplateFileConfig(
            fileName: Project.Name,
            fileExtension: "csproj"
        );
    }

    /// <summary>
    /// Applies settings from stereotypes in the Visual Studio designer.
    /// </summary>
    /// <returns>Whether or not there was a change.</returns>
    private bool ApplySettings(XDocument doc)
    {
        if (doc.ResolveProjectScheme() != VisualStudioProjectScheme.Sdk)
        {
            return false;
        }

        var hasChange = false;
        var project = ((IVisualStudioProjectTemplate)this).Project;

        hasChange |= SyncFrameworks(doc);

        var netCoreSettings = project.GetNETCoreSettings();
        if (netCoreSettings != null)
        {
            hasChange |= SyncProperty(doc, "Configurations", netCoreSettings.Configurations());
            hasChange |= SyncProperty(doc, "RuntimeIdentifiers", netCoreSettings.RuntimeIdentifiers());
            hasChange |= SyncProperty(doc, "UserSecretsId", netCoreSettings.UserSecretsId());
            hasChange |= SyncProperty(doc, "RootNamespace", netCoreSettings.RootNamespace());
            hasChange |= SyncProperty(doc, "AssemblyName", netCoreSettings.AssemblyName());
            hasChange |= SyncManageableBooleanProperty(doc, "ImplicitUsings", netCoreSettings.ImplicitUsings().Value);
            hasChange |= SyncManageableBooleanProperty(doc, "GenerateRuntimeConfigurationFiles", netCoreSettings.GenerateRuntimeConfigurationFiles().Value);
            hasChange |= SyncManageableBooleanProperty(doc, "GenerateDocumentationFile", netCoreSettings.GenerateDocumentationFile().Value);
        }

        if (project is CSharpProjectNETModel model &&
            model.HasNETSettings())
        {
            var netSettings = model.GetNETSettings();

            if (doc.Root!.Attribute("Sdk")!.Value != netSettings.SDK().Value)
            {
                doc.Root.Attribute("Sdk")!.Value = netSettings.SDK().Value;
                hasChange = true;
            }

            hasChange |= SyncProperty(doc, "OutputType", netSettings.OutputType().Value switch
            {
                "Class Library" => "Library",
                "Console Application" => "Exe",
                "Windows Application" => "WinExe",
                _ => null
            });
            hasChange |= SyncProperty(doc, "AzureFunctionsVersion", netSettings.AzureFunctionsVersion().Value, true);
            hasChange |= SyncProperty(doc, "Configurations", netSettings.Configurations());
            hasChange |= SyncProperty(doc, "RuntimeIdentifiers", netSettings.RuntimeIdentifiers());
            hasChange |= SyncProperty(doc, "UserSecretsId", netSettings.UserSecretsId());
            hasChange |= SyncProperty(doc, "RootNamespace", netSettings.RootNamespace());
            hasChange |= SyncProperty(doc, "AssemblyName", netSettings.AssemblyName());
            hasChange |= SyncManageableBooleanProperty(doc, "ImplicitUsings", netSettings.ImplicitUsings().Value);
            hasChange |= SyncManageableBooleanProperty(doc, "GenerateRuntimeConfigurationFiles", netSettings.GenerateRuntimeConfigurationFiles().Value);
            hasChange |= SyncManageableBooleanProperty(doc, "GenerateDocumentationFile", netSettings.GenerateDocumentationFile().Value);

            if (netSettings.SuppressWarnings() is null or "$(NoWarn)")
            {
                hasChange |= SyncProperty(doc, "NoWarn", null, removeIfNullOrEmpty: true);
            }
            else
            {
                hasChange |= SyncProperty(doc, "NoWarn", netSettings.SuppressWarnings());
            }
        }

        var projectOptions = project.GetCSharpProjectOptions();
        if (projectOptions != null)
        {
            hasChange |= SyncProperty(
                doc: doc,
                propertyName: "LangVersion",
                value: projectOptions.LanguageVersion().IsDefault()
                    ? null
                    : projectOptions.LanguageVersion().Value,
                removeIfNullOrEmpty: true);

            if (projectOptions.Nullable()?.Value == "(unspecified)")
            {
                hasChange |= SyncProperty(doc, "Nullable", null, removeIfNullOrEmpty: true);
            }
            else if (!string.IsNullOrWhiteSpace(projectOptions.Nullable()?.Value))
            {
                hasChange |= SyncProperty(doc, "Nullable", projectOptions.Nullable().Value);
            }
            else if (projectOptions.NullableEnabled())
            {
                // NullableEnabled() was the old property which is just a checkbox, we fall
                // back to it if Nullable() is unset.
                hasChange |= SyncProperty(doc, "Nullable", "enable");
            }
        }

        return hasChange;
    }

    /// <summary>
    /// For when <paramref name="value"/> is one of the following:
    /// <list type="table">
    /// <item>
    /// <term><see langword="null"/></term>
    /// <description>The property's value us "unmanaged" by Intent and should not be changed, added, or removed.</description>
    /// </item>
    /// <item>
    /// <term>"(unspecified)"</term>
    /// <description>The property should be removed from the <c>.csproj</c> file.</description>
    /// </item>
    /// <item>
    /// <term><see langword="false"/> / <c>disable</c></term>
    /// <description>The property's value should be set to <c>false</c>. / <c>disable</c></description>
    /// </item>
    /// <item>
    /// <term><see langword="true"/> / <c>enable</c></term>
    /// <description>The property's value should be set to <c>true</c> / <c>enable</c>.</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <returns>True if there was a change.</returns>
    private static bool SyncManageableBooleanProperty(XDocument doc, string propertyName, string value)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            value is not "(unspecified)" &&
            value is not "enable" &&
            value is not "disable" &&
            value is not "true" &&
            value is not "false")
        {
            throw new ArgumentOutOfRangeException(nameof(value), value);
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return SyncProperty(doc, propertyName, value == "(unspecified)" ? null : value);
    }

    /// <returns>True if there was a change.</returns>
    private static bool SyncProperty(XDocument doc, string propertyName, string value, bool removeIfNullOrEmpty = false)
    {
        var element = GetPropertyGroupElement(doc, propertyName);
        if (string.IsNullOrWhiteSpace(value))
        {
            if (!removeIfNullOrEmpty ||
                element == null)
            {
                return false;
            }

            element.Remove();
            return true;
        }

        if (element == null)
        {
            var propertyGroupElement = GetPropertyGroupElement(doc, "TargetFramework")?.Parent ??
                                       GetPropertyGroupElement(doc, "TargetFrameworks")?.Parent;
            if (propertyGroupElement == null)
            {
                throw new Exception("Could not determine target property group element.");
            }

            element = new XElement(propertyName);
            propertyGroupElement.Add(element);
        }

        if (element.Value == value)
        {
            return false;
        }

        element.Value = value;
        return true;
    }

    /// <returns>True if there was a change.</returns>
    private bool SyncFrameworks(XDocument doc)
    {
        var targetFrameworks = GetTargetFrameworks().ToArray();
        if (targetFrameworks.Length == 1 && targetFrameworks[0] == "unspecified")
        {
            // User has chosen "(unspecified)" in the Visual Studio designer, useful for
            // scenarios like when a "Directory.Build.props" is being used to set the
            // value.
            return false;
        }

        var element = GetPropertyGroupElement(doc, "TargetFramework") ??
                      GetPropertyGroupElement(doc, "TargetFrameworks");
        if (element == null)
        {
            return false;
        }

        var elementValue = string.Join(";", targetFrameworks.OrderBy(x => x));
        if (element.Value == elementValue)
        {
            return false;
        }

        var elementName = targetFrameworks.Count() == 1
            ? "TargetFramework"
            : "TargetFrameworks";

        element.ReplaceWith(XElement.Parse($"<{elementName}>{elementValue}</{elementName}>"));

        return true;
    }

    private static XElement GetPropertyGroupElement(XDocument doc, string name)
    {
        var (prefix, namespaceManager, _) = doc.GetNamespaceManager();

        return doc.XPathSelectElement($"/{prefix}:Project/{prefix}:PropertyGroup/{prefix}:{name}", namespaceManager);
    }
}