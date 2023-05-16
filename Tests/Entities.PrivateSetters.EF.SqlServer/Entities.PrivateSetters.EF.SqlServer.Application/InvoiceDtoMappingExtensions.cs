using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Entities.PrivateSetters.EF.SqlServer.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Application
{
    public static class InvoiceDtoMappingExtensions
    {
        public static InvoiceDto MapToInvoiceDto(this Invoice projectFrom, IMapper mapper)
        {
            return mapper.Map<InvoiceDto>(projectFrom);
        }

        public static List<InvoiceDto> MapToInvoiceDtoList(this IEnumerable<Invoice> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToInvoiceDto(mapper)).ToList();
        }
    }
}