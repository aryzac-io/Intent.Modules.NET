using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryInterface", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Common.Interfaces
{
    public interface IQuery
    {

    }
}