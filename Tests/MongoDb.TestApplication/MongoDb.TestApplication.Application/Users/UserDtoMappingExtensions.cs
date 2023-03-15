using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Users
{
    public static class UserDtoMappingExtensions
    {
        public static UserDto MapToUserDto(this User projectFrom, IMapper mapper)
        {
            return mapper.Map<UserDto>(projectFrom);
        }

        public static List<UserDto> MapToUserDtoList(this IEnumerable<User> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToUserDto(mapper)).ToList();
        }
    }
}