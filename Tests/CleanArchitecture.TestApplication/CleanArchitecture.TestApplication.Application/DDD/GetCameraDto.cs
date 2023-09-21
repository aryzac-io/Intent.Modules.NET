using System;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.DDD
{
    public class GetCameraDto : IMapFrom<Camera>
    {
        public GetCameraDto()
        {
            IdemiaId = null!;
        }

        public Guid Id { get; set; }
        public string IdemiaId { get; set; }

        public static GetCameraDto Create(Guid id, string idemiaId)
        {
            return new GetCameraDto
            {
                Id = id,
                IdemiaId = idemiaId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Camera, GetCameraDto>();
        }
    }
}