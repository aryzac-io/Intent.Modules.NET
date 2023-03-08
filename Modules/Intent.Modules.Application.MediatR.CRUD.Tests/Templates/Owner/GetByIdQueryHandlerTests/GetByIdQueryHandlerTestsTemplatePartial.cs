using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
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

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Owner.GetByIdQueryHandlerTests;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class GetByIdQueryHandlerTestsTemplate : CSharpTemplateBase<QueryModel>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.Owner.GetByIdQueryHandlerTests";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public GetByIdQueryHandlerTestsTemplate(IOutputTarget outputTarget, QueryModel model) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.AutoFixture);
        AddNugetDependency(NugetPackages.FluentAssertions);
        AddNugetDependency(NugetPackages.MicrosoftNetTestSdk);
        AddNugetDependency(NugetPackages.NSubstitute);
        AddNugetDependency(NugetPackages.Xunit);
        AddNugetDependency(NugetPackages.XunitRunnerVisualstudio);

        AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
        AddTypeSource(QueryModelsTemplate.TemplateId);
        AddTypeSource(TemplateFulfillingRoles.Application.Contracts.Dto);

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddClass($"{Model.Name}HandlerTests")
            .OnBuild(file =>
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
                file.AddUsing("AutoMapper");

                var dtoModel = Model.TypeReference.Element.AsDTOModel();
                var domainElement = Model.Mapping.Element.AsClassModel();
                var domainElementName = domainElement.Name.ToPascalCase();
                var domainElementIdName = domainElement.GetEntityIdAttribute().IdName;
                var queryIdFieldName = Model.Properties.GetEntityIdField(domainElement).Name;

                var priClass = file.Classes.First();
                priClass.AddField("IMapper", "_mapper", prop => prop.PrivateReadOnly());
                priClass.AddConstructor(ctor =>
                {
                    ctor.AddStatement(new CSharpInvocationStatement("var mapperConfiguration = new MapperConfiguration")
                        .AddArgument(new CSharpLambdaBlock("config")
                            .AddStatement($"config.AddMaps(typeof({this.GetQueryHandlerName(Model)}));"))
                        .WithArgumentsOnNewLines());
                    ctor.AddStatement("_mapper = mapperConfiguration.CreateMapper();");
                });

                priClass.AddMethod("Task", $"Handle_WithValidQuery_Retrieves{domainElementName}", method =>
                {
                    method.Async();
                    method.AddAttribute("Theory");
                    method.AddAttribute("MemberData(nameof(GetTestData))");
                    method.AddParameter(GetTypeName(domainElement.InternalElement), "testEntity");
                    method.AddStatements($@"
        // Arrange
        var expectedDto = CreateExpected{dtoModel.Name.ToPascalCase()}(testEntity);
        
        var testQuery = new {this.GetQueryModelsName(Model)} {{ {queryIdFieldName} = testEntity.{domainElementIdName} }};
        var repository = Substitute.For<{this.GetEntityRepositoryInterfaceName(domainElement)}>();
        repository.FindByIdAsync(testQuery.{queryIdFieldName}, CancellationToken.None).Returns(Task.FromResult(testEntity));

        var sut = new {this.GetQueryHandlerName(Model)}(repository, _mapper);

        // Act
        var result = await sut.Handle(testQuery, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);");
                });

                priClass.AddMethod("Task", "Handle_WithInvalidIdQuery_ReturnsEmptyResult", method =>
                {
                    var idFieldName = Model.Properties.GetEntityIdField(domainElement).Name.ToCSharpIdentifier();

                    method.Async();
                    method.AddAttribute("Fact");
                    method.AddStatements($@"
        // Arrange
        var fixture = new Fixture();
        var query = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        
        var repository = Substitute.For<{this.GetEntityRepositoryInterfaceName(domainElement)}>();
        repository.FindByIdAsync(query.{idFieldName}, CancellationToken.None).Returns(Task.FromResult<{GetTypeName(domainElement.InternalElement)}>(default));

        var sut = new {this.GetQueryHandlerName(Model)}(repository, _mapper);
        
        // Act
        var result = await sut.Handle(query, CancellationToken.None);
        
        // Assert
        result.Should().Be(null);");
                });

                priClass.AddMethod("IEnumerable<object[]>", "GetTestData", method =>
                {
                    method.Static();
                    method.AddStatements($@"var fixture = new Fixture();");

                    this.RegisterDomainEventBaseFixture(method, domainElement);

                    method.AddStatement($@"yield return new object[] {{ fixture.Create<{GetTypeName(domainElement.InternalElement)}>() }};");
                });

                this.AddDomainToDtoMappingMethods(priClass, domainElement, dtoModel);
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