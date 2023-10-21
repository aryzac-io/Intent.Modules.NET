using System;
using Intent.RoslynWeaver.Attributes;

namespace Standard.AspNetCore.TestApplication.Domain.Entities
{
    public class InvoiceLine
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid InvoiceId { get; set; }
    }
}