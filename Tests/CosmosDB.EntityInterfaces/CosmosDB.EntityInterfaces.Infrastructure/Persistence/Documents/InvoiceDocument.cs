using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class InvoiceDocument : IInvoiceDocument, ICosmosDBDocument<IInvoice, Invoice, InvoiceDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; } = default!;
        public string ClientIdentifier { get; set; } = default!;
        public DateTime Date { get; set; }
        public string Number { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
        public DateTimeOffset CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public List<LineItemDocument> LineItems { get; set; } = default!;
        IReadOnlyList<ILineItemDocument> IInvoiceDocument.LineItems => LineItems;
        public InvoiceLogoDocument InvoiceLogo { get; set; } = default!;
        IInvoiceLogoDocument IInvoiceDocument.InvoiceLogo => InvoiceLogo;

        public Invoice ToEntity(Invoice? entity = default)
        {
            entity ??= ReflectionHelper.CreateNewInstanceOf<Invoice>();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.ClientIdentifier = ClientIdentifier ?? throw new Exception($"{nameof(entity.ClientIdentifier)} is null");
            entity.Date = Date;
            entity.Number = Number ?? throw new Exception($"{nameof(entity.Number)} is null");
            entity.CreatedBy = CreatedBy ?? throw new Exception($"{nameof(entity.CreatedBy)} is null");
            entity.CreatedDate = CreatedDate;
            entity.UpdatedBy = UpdatedBy;
            entity.UpdatedDate = UpdatedDate;
            entity.LineItems = LineItems.Select(x => x.ToEntity()).ToList();
            entity.InvoiceLogo = InvoiceLogo.ToEntity() ?? throw new Exception($"{nameof(entity.InvoiceLogo)} is null");

            return entity;
        }

        public InvoiceDocument PopulateFromEntity(IInvoice entity)
        {
            Id = entity.Id;
            ClientIdentifier = entity.ClientIdentifier;
            Date = entity.Date;
            Number = entity.Number;
            CreatedBy = entity.CreatedBy;
            CreatedDate = entity.CreatedDate;
            UpdatedBy = entity.UpdatedBy;
            UpdatedDate = entity.UpdatedDate;
            LineItems = entity.LineItems.Select(x => LineItemDocument.FromEntity(x)!).ToList();
            InvoiceLogo = InvoiceLogoDocument.FromEntity(entity.InvoiceLogo)!;

            return this;
        }

        public static InvoiceDocument? FromEntity(IInvoice? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new InvoiceDocument().PopulateFromEntity(entity);
        }
    }
}