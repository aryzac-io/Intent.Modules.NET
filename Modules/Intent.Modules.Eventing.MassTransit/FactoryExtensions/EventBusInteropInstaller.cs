using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EventBusInteropInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.MassTransit.EventBusInteropInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            InstallMessageBusForServiceContractDispatch(application);
            InstallMessageBusForMediatRDispatch(application);
        }

        private void InstallMessageBusForMediatRDispatch(IApplication application)
        {
            if (!IsTransactionalOutboxPatternSelected(application))
            {
                return;
            }

            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Application.DependencyInjection.DependencyInjection"); // Replace with Role later.
            if (template == null)
            {
                return;
            }

            template.CSharpFile.AfterBuild(file =>
            {
                var priClass = file.Classes.First();
                var method = priClass.FindMethod("AddApplication");
                var meditarConfigLambda = (CSharpInvocationStatement)method.FindStatement(stmt => stmt.HasMetadata("mediatr-config"));
                if (meditarConfigLambda == null)
                {
                    return;
                }
                var mediatorConfig = (CSharpLambdaBlock)(meditarConfigLambda.Statements.FirstOrDefault());
                var statementToMove = mediatorConfig.Statements.FirstOrDefault(stmt => stmt.GetText("").Contains("EventBusPublishBehaviour"));
                if (statementToMove == null)
                {
                    return;
                }
                statementToMove.Remove();
                var unitOfWork = mediatorConfig.Statements.FirstOrDefault(stmt => stmt.GetText("").Contains("UnitOfWorkBehaviour"));
                unitOfWork.InsertBelow(statementToMove);
            }, 1000);
        }

        private void InstallMessageBusForServiceContractDispatch(IApplication application)
        {
            if (!IsTransactionalOutboxPatternSelected(application))
            {
                return;
            }

            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Application.Services.Controllers));
            foreach (var template in templates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var priClass = file.Classes.First();
                    foreach (var method in priClass.Methods)
                    {
                        var statementToMove = method.FindStatement(stmt => stmt.HasMetadata("eventbus-flush"));
                        if (statementToMove == null)
                        {
                            continue;
                        }
                        statementToMove.Remove();
                        method.FindStatement(p => p.HasMetadata("service-contract-dispatch"))
                            ?.InsertBelow(statementToMove);
                    }
                }, 1000);
            }
        }

        private bool IsTransactionalOutboxPatternSelected(IApplication application)
        {
            return application.Settings.GetEventingSettings()?.OutboxPattern()?.IsEntityFramework() == true;
        }
    }
}