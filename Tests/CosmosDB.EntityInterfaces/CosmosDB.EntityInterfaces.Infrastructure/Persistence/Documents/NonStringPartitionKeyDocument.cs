using System;
using System.Globalization;
using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class NonStringPartitionKeyDocument : INonStringPartitionKeyDocument, ICosmosDBDocument<INonStringPartitionKey, NonStringPartitionKey, NonStringPartitionKeyDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? ICosmosDBDocument.PartitionKey
        {
            get => PartInt;
            set => PartInt = value!;
        }
        public string Id { get; set; } = default!;
        public string PartInt { get; set; }
        public string Name { get; set; } = default!;

        public NonStringPartitionKey ToEntity(NonStringPartitionKey? entity = default)
        {
            entity ??= new NonStringPartitionKey();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.PartInt = int.Parse(PartInt, CultureInfo.InvariantCulture);
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public NonStringPartitionKeyDocument PopulateFromEntity(INonStringPartitionKey entity)
        {
            Id = entity.Id;
            PartInt = entity.PartInt.ToString(CultureInfo.InvariantCulture);
            Name = entity.Name;

            return this;
        }

        public static NonStringPartitionKeyDocument? FromEntity(INonStringPartitionKey? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new NonStringPartitionKeyDocument().PopulateFromEntity(entity);
        }
    }
}