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
    internal class ProductDocument : IProductDocument, ICosmosDBDocument<IProduct, Product, ProductDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<string> CategoriesIds { get; set; } = default!;
        IReadOnlyList<string> IProductDocument.CategoriesIds => CategoriesIds;

        public Product ToEntity(Product? entity = default)
        {
            entity ??= new Product();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.CategoriesIds = CategoriesIds ?? throw new Exception($"{nameof(entity.CategoriesIds)} is null");

            return entity;
        }

        public ProductDocument PopulateFromEntity(IProduct entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            CategoriesIds = entity.CategoriesIds.ToList();

            return this;
        }

        public static ProductDocument? FromEntity(IProduct? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ProductDocument().PopulateFromEntity(entity);
        }
    }
}