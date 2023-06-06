using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Interfaces.ServiceDispatch
{
    public interface IServiceDispatchWithoutImplementationService : IDisposable
    {
        void Mutation(string param);
        Task MutationAsync(CancellationToken cancellationToken = default);
        Task MutationAsync(string param, CancellationToken cancellationToken = default);
        string Query(string param);
        string Query();
        Task<string> QueryAsync(CancellationToken cancellationToken = default);
        Task<string> QueryAsync(string param, CancellationToken cancellationToken = default);
        void Mutation();
    }
}