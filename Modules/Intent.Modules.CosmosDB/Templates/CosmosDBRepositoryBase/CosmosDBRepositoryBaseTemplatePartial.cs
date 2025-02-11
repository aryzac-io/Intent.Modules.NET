using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates.CosmosDBRepositoryBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBRepositoryBaseTemplate : CSharpTemplateBase<IList<ClassModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBRepositoryBase";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBRepositoryBaseTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
            AddNugetDependency(NugetDependencies.IEvangelistAzureCosmosRepository);
            AddNugetDependency(NugetDependencies.NewtonsoftJson);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.Azure.Cosmos")
                .AddClass("CosmosDBRepositoryBase", @class =>
                {
                    @class
                        .Internal()
                        .Abstract()
                        .AddGenericParameter("TDomain", out var tDomain);

                    var tDomainState = tDomain;
                    if (createEntityInterfaces)
                    {
                        @class
                            .AddGenericParameter("TDomainState", out tDomainState);
                    }

                    @class
                        .AddGenericParameter("TDocument", out var tDocument)
                        .AddGenericParameter("TDocumentInterface", out var tDocumentInterface);

                    var selectGenericTypeArgument = createEntityInterfaces
                        ? $"<{tDocument}, {tDomain}>"
                        : string.Empty;

                    @class
                        .ImplementsInterface($"{this.GetCosmosDBRepositoryInterfaceName()}<{tDomain}, {tDocumentInterface}>")
                        .AddGenericTypeConstraint(tDomain, c => c
                            .AddType("class"));

                    if (createEntityInterfaces)
                    {
                        @class
                            .AddGenericTypeConstraint(tDomainState, c => c
                                .AddType("class")
                                .AddType(tDomain));
                    }

                    var tDomainStateConstraint = createEntityInterfaces
                        ? $", {tDomainState}"
                        : string.Empty;
                    @class
                        .AddGenericTypeConstraint(tDocument, c => c
                            .AddType($"{this.GetCosmosDBDocumentOfTInterfaceName()}<{tDomain}{tDomainStateConstraint}, {tDocument}>")
                            .AddType(tDocumentInterface)
                            .AddType("new()"))
                        ;

                    @class.AddConstructor(ctor =>
                    {
                        ctor.Protected();
                        ctor.AddParameter(this.GetCosmosDBUnitOfWorkName(), "unitOfWork",
                            p => p.IntroduceReadonlyField());
                        ctor.AddParameter(UseType($"Microsoft.Azure.CosmosRepository.IRepository<{tDocument}>"),
                            "cosmosRepository", p => p.IntroduceReadonlyField());
                        ctor.AddParameter("string", "idFieldName", p => p.IntroduceReadonlyField());
                    });

                    @class.AddProperty(this.GetCosmosDBUnitOfWorkInterfaceName(), "UnitOfWork", p => p
                        .WithoutSetter()
                        .Getter.WithExpressionImplementation("_unitOfWork")
                    );

                    @class.AddMethod("void", "Add", m => m
                        .AddParameter(tDomain, "entity")
                        .AddStatement("_unitOfWork.Track(entity);", s => s.SeparatedFromPrevious())
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.AddMetadata(MetadataNames.EnqueueStatement, true);
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement($"var document = new {tDocument}().PopulateFromEntity(entity);", c => c.AddMetadata(MetadataNames.DocumentDeclarationStatement, true))
                                    .AddStatement(
                                        "await _cosmosRepository.CreateAsync(document, cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    @class.AddMethod("void", "Update", m => m
                        .AddParameter(tDomain, "entity")
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.AddMetadata(MetadataNames.EnqueueStatement, true);
                            invocation.SeparatedFromPrevious();
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement($"var document = new {tDocument}().PopulateFromEntity(entity);", c => c.AddMetadata(MetadataNames.DocumentDeclarationStatement, true))
                                    .AddStatement(
                                        "await _cosmosRepository.UpdateAsync(document, cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    @class.AddMethod("void", "Remove", m => m
                        .AddParameter(tDomain, "entity")
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.AddMetadata(MetadataNames.EnqueueStatement, true);
                            invocation.SeparatedFromPrevious();
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement($"var document = new {tDocument}().PopulateFromEntity(entity);", c => c.AddMetadata(MetadataNames.DocumentDeclarationStatement, true))
                                    .AddStatement(
                                        "await _cosmosRepository.DeleteAsync(document, cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", m => m
                        .Async()
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement("var documents = await _cosmosRepository.GetAsync(_ => true, cancellationToken);", c => c.AddMetadata(MetadataNames.DocumentsDeclarationStatement, true))
                        .AddStatement($"var results = documents.Select{selectGenericTypeArgument}(document => document.ToEntity()).ToList();")
                        .AddStatement("Track(results);")
                        .AddStatement("return results;", s => s.SeparatedFromPrevious())
                    );


                    @class.AddMethod($"Task<{tDomain}?>", "FindByIdAsync", m => m
                        .Async()
                        .Protected()
                        .AddParameter("string", "id")
                        .AddParameter("string?", "partitionKey", p => p.WithDefaultValue("default"))
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddTryBlock(tryBlock =>
                        {
                            tryBlock
                                .AddStatement(
                                    "var document = await _cosmosRepository.GetAsync(id, partitionKey, cancellationToken: cancellationToken);",
                                    c => c.AddMetadata(MetadataNames.DocumentDeclarationStatement, true))
                                .AddStatement("var entity = document.ToEntity();")
                                .AddStatement("Track(entity);")
                                .AddStatement("return entity;", s => s.SeparatedFromPrevious());
                        })
                        .AddCatchBlock(UseType("Microsoft.Azure.Cosmos.CosmosException"), "ex", c =>
                        {
                            c.WithWhenExpression($"ex.StatusCode == {UseType("System.Net.HttpStatusCode")}.NotFound");
                            c.AddStatement("return null;");
                        })
                    );

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method
                            .AddStatement(
                                "var documents = await _cosmosRepository.GetAsync(AdaptFilterPredicate(filterExpression), cancellationToken);"
                                , c => c.AddMetadata(MetadataNames.DocumentsDeclarationStatement, true))
                            .AddStatement($"var results = documents.Select{selectGenericTypeArgument}(document => document.ToEntity()).ToList();")
                            .AddStatement("Track(results);")
                            .AddStatement("return results;", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod($"Task<{this.GetPagedResultInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddStatement("return await FindAllAsync(_ => true, pageNo, pageSize, cancellationToken);");
                    });

                    @class.AddMethod($"Task<{this.GetPagedResultInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        var tDomainStateGenericTypeArgument = createEntityInterfaces
                            ? $", {tDomainState}"
                            : string.Empty;
                        method
                            .AddStatement(
                                "var pagedDocuments = await _cosmosRepository.PageAsync(AdaptFilterPredicate(filterExpression), pageNo, pageSize, true, cancellationToken);",
                                c => c.AddMetadata(MetadataNames.PagedDocumentsDeclarationStatement, true))
                            .AddStatement("Track(pagedDocuments.Items.Select(document => document.ToEntity()));")
                            .AddStatement($"return new {this.GetCosmosPagedListName()}<{tDomain}{tDomainStateGenericTypeArgument}, {tDocument}>(pagedDocuments, pageNo, pageSize);", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindByIdsAsync", m => m
                        .AddParameter("IEnumerable<string>", "ids")
                        .Async()
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement(
                            @"var queryDefinition = new QueryDefinition($""SELECT * from c WHERE ARRAY_CONTAINS(@ids, c.{_idFieldName})"")
                .WithParameter(""@ids"", ids);",
                            c => c.AddMetadata(MetadataNames.QueryDefinitionDeclarationStatement, true))
                        .AddStatement(
                            "var documents = await _cosmosRepository.GetByQueryAsync(queryDefinition, cancellationToken);")
                        .AddStatement($"var results = documents.Select{selectGenericTypeArgument}(document => document.ToEntity()).ToList();")
                        .AddStatement("Track(results);")
                        .AddStatement("return results;", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Expression<Func<{tDocument}, bool>>", "AdaptFilterPredicate", method =>
                    {
                        method
                            .Private()
                            .Static()
                            .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "expression")
                            .WithComments(new[]
                            {
                                "/// <summary>",
                                $"/// Adapts a <typeparamref name=\"{tDocumentInterface}\"/> predicate to a <typeparamref name=\"{tDocument}\"/> predicate.",
                                "/// </summary>"
                            });

                        method.AddStatement("var beforeParameter = expression.Parameters.Single();");
                        method.AddStatement($"var afterParameter = Expression.Parameter(typeof({tDocument}), beforeParameter.Name);");
                        method.AddStatement("var visitor = new SubstitutionExpressionVisitor(beforeParameter, afterParameter);");
                        method.AddStatement($"return Expression.Lambda<Func<{tDocument}, bool>>(visitor.Visit(expression.Body)!, afterParameter);");

                    });

                    @class.AddMethod("void", "Track", method =>
                    {
                        method.AddParameter($"IEnumerable<{tDomain}>", "items");

                        method.AddForEachStatement("item", "items", stmt => stmt.AddStatement("_unitOfWork.Track(item);"));

                    });

                    @class.AddMethod("void", "Track", method =>
                    {
                        method.AddParameter($"{tDomain}", "item");
                        method.AddStatement("_unitOfWork.Track(item);");
                    });


                    @class.AddNestedClass("SubstitutionExpressionVisitor", nestClass =>
                    {
                        nestClass
                            .Private()
                            .WithBaseType("ExpressionVisitor");
                        nestClass.AddConstructor(ctor =>
                        {
                            ctor
                                .AddParameter("Expression", "before", p => p.IntroduceReadonlyField())
                                .AddParameter("Expression", "after", p => p.IntroduceReadonlyField());
                        });

                        nestClass.AddMethod("Expression?", "Visit", method =>
                        {
                            method
                                .Override()
                                .AddParameter("Expression?", "node");

                            method.AddStatement("return node == _before ? _after : base.Visit(node);");
                        });
                    });
                });
        }

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            this.ApplyAppSetting("RepositoryOptions", new
            {
                CosmosConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                DatabaseId = ExecutionContext.GetApplicationConfig().Name,
                ContainerId = "Container"
            });
            ExecutionContext.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.CosmosDb.Name)
                .WithProperty(Infrastructure.CosmosDb.Property.ConnectionStringSettingPath, "RepositoryOptions:CosmosConnectionString"));
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