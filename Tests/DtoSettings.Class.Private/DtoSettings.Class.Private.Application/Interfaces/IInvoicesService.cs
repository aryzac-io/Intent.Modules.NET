using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DtoSettings.Class.Private.Application.Invoices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace DtoSettings.Class.Private.Application.Interfaces
{
    public interface IInvoicesService : IDisposable
    {
        Task<Guid> CreateInvoice(InvoiceCreateDto dto, CancellationToken cancellationToken = default);
        Task<InvoiceDto> FindInvoiceById(Guid id, CancellationToken cancellationToken = default);
        Task<List<InvoiceDto>> FindInvoices(CancellationToken cancellationToken = default);
        Task UpdateInvoice(Guid id, InvoiceUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteInvoice(Guid id, CancellationToken cancellationToken = default);
    }
}