using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class RegionDocument : IRegionDocument, ICosmosDBDocument<Region, RegionDocument>
    {
        private string? _type;
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<CountryDocument> Countries { get; set; } = default!;
        IReadOnlyList<ICountryDocument> IRegionDocument.Countries => Countries;

        public Region ToEntity(Region? entity = default)
        {
            entity ??= new Region();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Countries = Countries.Select(x => x.ToEntity()).ToList();

            return entity;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }

        public RegionDocument PopulateFromEntity(Region entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Countries = entity.Countries.Select(x => CountryDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static RegionDocument? FromEntity(Region? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new RegionDocument().PopulateFromEntity(entity);
        }
    }
}