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

        AddTypeSource(TemplateRoles.Application.Contracts.Dto);

        CSharpFile = new CSharpFile($"{this.GetNamespace()}", $"{this.GetFolderPath()}")
            .AddClass($"{Model.Name}HandlerTests")
            .AfterBuild(file =>
            {
                var facade = new CommandHandlerFacade(this, model);
                AddUsingDirectives(file);
                facade.AddHandlerConstructorMockUsings();

                var priClass = file.Classes.First();
                AddSuccessfulResultTestData(priClass, facade);
                AddSuccessfulHandlerTest(priClass, facade);

                this.AddCommandAssertionMethods(Model);
            }, 1);
    }

    private bool? _canRunTemplate;

    public override bool CanRunTemplate()
    {
        if (_canRunTemplate.HasValue)
        {
            return _canRunTemplate.Value;
        }

        var template = this.GetCommandHandlerTemplate(Model, trackDependency: false);
        if (template is null)
        {
            _canRunTemplate = false;
        }
        else if (StrategyFactory.GetMatchedCommandStrategy(template) is CreateImplementationStrategy strategy && strategy.IsMatch())
        {
            _canRunTemplate = Model.GetClassModel()?.IsAggregateRoot() == true && Model.GetClassModel().InternalElement.Package.HasStereotype("Relational Database");
        }
        else
        {
            _canRunTemplate = false;
        }

        return _canRunTemplate.Value;
    }

    private static void AddSuccessfulResultTestData(CSharpClass priClass, CommandHandlerFacade facade)
    {
        priClass.AddMethod("IEnumerable<object[]>", "GetSuccessfulResultTestData", method =>
        {
            method.Static();
            method.AddStatements(facade.Get_ProduceSingleCommand_TestDataStatements());
            method.AddStatements(facade.Get_ProduceCommandWithNullableFields_TestDataStatements());
        });
    }

    private static void AddSuccessfulHandlerTest(CSharpClass priClass, CommandHandlerFacade facade)
    {
        priClass.AddMethod("Task", $"Handle_WithValidCommand_Adds{facade.SingularTargetDomainName}ToRepository", method =>
        {
            method.Async();
            method.AddAttribute("Theory");
            method.AddAttribute("MemberData(nameof(GetSuccessfulResultTestData))");
            method.AddParameter(facade.CommandTypeName, "testCommand");

            method.AddStatement("// Arrange");
            method.AddStatements(facade.GetCommandHandlerConstructorParameterMockStatements());
            method.AddStatements(facade.GetAggregateDomainRepositoryUnitOfWorkMockingStatements());
            method.AddStatements(facade.GetCommandHandlerConstructorSutStatement());

            method.AddStatement(string.Empty);
            method.AddStatement("// Act");
            method.AddStatements(facade.GetSutHandleInvocationStatement("testCommand"));

            method.AddStatement(string.Empty);
            method.AddStatement("// Assert");
            method.AddStatements(facade.GetAggregateDomainRepositorySaveChangesAssertionStatement());
            method.AddStatements(facade.GetCommandCompareToNewAddedDomainAssertionStatement("testCommand"));
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