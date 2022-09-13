using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IntegrationHttpClientTestSuite.IntentGenerated.Contracts.Invoices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "1.0")]

namespace IntegrationHttpClientTestSuite.IntentGenerated.Contracts
{
    public interface IInvoiceService : IDisposable
    {
        Task Create(InvoiceCreateDTO dto, CancellationToken cancellationToken = default);
        Task<InvoiceDTO> FindById(Guid id, CancellationToken cancellationToken = default);
        Task<List<InvoiceDTO>> FindAll(CancellationToken cancellationToken = default);
        Task Update(Guid id, InvoiceUpdateDTO dto, CancellationToken cancellationToken = default);
        Task Delete(Guid id, CancellationToken cancellationToken = default);
        Task<InvoiceDTO> QueryParamOp(string param1, int param2, CancellationToken cancellationToken = default);
        Task HeaderParamOp(string param1, CancellationToken cancellationToken = default);
        Task FormParamOp(string param1, int param2, CancellationToken cancellationToken = default);
        Task RouteParamOp(string param1, CancellationToken cancellationToken = default);
        Task BodyParamOp(InvoiceDTO param1, CancellationToken cancellationToken = default);
    }
}