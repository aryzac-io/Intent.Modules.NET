using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Assertions.AssertionClass;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Extensions.RepositoryExtensions;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Owner.CreateCommandHandlerTests;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class CreateCommandHandlerTestsTemplate : CSharpTemplateBase<CommandModel>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public CreateCommandHandlerTestsTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.AutoFixture);
        AddNugetDependency(NugetPackages.FluentAssertions);
        AddNugetDependency(NugetPackages.MicrosoftNetTestSdk);
        AddNugetDependency(NugetPackages.NSubstitute);
        AddNugetDependency(NugetPackages.Xunit);
        AddNugetDependency(NugetPackages.XunitRunnerVisualstudio);

        AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
        AddTypeSource(CommandModelsTemplate.TemplateId);
        AddTypeSource(TemplateFulfillingRoles.Application.Contracts.Dto);

        CSharpFile = new CSharpFile($"{this.GetNamespace()}", $"{this.GetFolderPath()}")
            .AddClass($"{Model.Name}HandlerTests")
            .AfterBuild(file =>
            {
                AddUsingDirectives(file);

                var priClass = file.Classes.First();
                AddSuccessfulResultTestData(priClass);
                AddSuccessfulHandlerTest(priClass);
            })
            .AfterBuild(file =>
            {
                AddAssertionMethods();
            }, 1);
    }

    private void AddSuccessfulResultTestData(CSharpClass priClass)
    {
        priClass.AddMethod("IEnumerable<object[]>", "GetSuccessfulResultTestData", method =>
        {
            method.Static();
            method.AddStatements($@"var fixture = new Fixture();
        yield return new object[] {{ fixture.Create<{GetTypeName(Model.InternalElement)}>() }};");

            foreach (var property in Model.Properties
                         .Where(p => p.TypeReference.IsNullable && p.Mapping?.Element?.AsAssociationEndModel()?.Element?.AsClassModel()?.IsAggregateRoot() == false))
            {
                method.AddStatement("");
                method.AddStatements($@"
        fixture = new Fixture();
        fixture.Customize<{GetTypeName(Model.InternalElement)}>(comp => comp.Without(x => x.{property.Name}));
        yield return new object[] {{ fixture.Create<{GetTypeName(Model.InternalElement)}>() }};");
            }
        });
    }
    
    private void AddSuccessfulHandlerTest(CSharpClass priClass)
    {
        var facade = new CommandHandlerFacade(this, Model);
        
        priClass.AddMethod("Task", $"Handle_WithValidCommand_Adds{facade.DomainClassName}ToRepository", method =>
        {
            method.Async();
            method.AddAttribute("Theory");
            method.AddAttribute("MemberData(nameof(GetSuccessfulResultTestData))");
            method.AddParameter(GetTypeName(Model.InternalElement), "testCommand");
            
            method.AddStatement("// Arrange");
            facade.AddHandlerConstructorMockUsings();
            method.AddStatements(facade.GetCommandHandlerConstructorParameterMockStatements());
            method.AddStatements(facade.GetRepositoryUnitOfWorkMockingStatements());
            method.AddStatement(string.Empty);
            method.AddStatements(facade.GetCommandHandlerConstructorSutStatement());
            
            method.AddStatement(string.Empty);
            method.AddStatement("// Act");
            method.AddStatements(facade.GetSutHandleInvocationStatement("testCommand"));
            
            method.AddStatement(string.Empty);
            method.AddStatement("// Assert");
            method.AddStatements(facade.GetRepositorySaveChangesAssertionStatement());
            method.AddStatements(facade.GetDomainAssertionStatement("testCommand"));
        });
    }

    private void AddUsingDirectives(CSharpFile file)
    {
        file.AddUsing("System");
        file.AddUsing("System.Collections.Generic");
        file.AddUsing("System.Linq");
        file.AddUsing("System.Threading");
        file.AddUsing("System.Threading.Tasks");
        file.AddUsing("AutoFixture");
        file.AddUsing("FluentAssertions");
        file.AddUsing("NSubstitute");
        file.AddUsing("Xunit");

        var repoExtTemplate = ExecutionContext.FindTemplateInstance<IClassProvider>(TemplateDependency.OnTemplate(RepositoryExtensionsTemplate.TemplateId));
        if (repoExtTemplate != null)
        {
            file.AddUsing(repoExtTemplate.Namespace);
        }
    }

    private void AddAssertionMethods()
    {
        if (Model?.Mapping?.Element?.IsClassModel() != true)
        {
            return;
        }

        var domainModel = Model.Mapping.Element.AsClassModel();
        var template = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(
            TemplateDependency.OnModel(AssertionClassTemplate.TemplateId, domainModel));

        template?.AddAssertionMethods(template.CSharpFile.Classes.First(), Model, domainModel);
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