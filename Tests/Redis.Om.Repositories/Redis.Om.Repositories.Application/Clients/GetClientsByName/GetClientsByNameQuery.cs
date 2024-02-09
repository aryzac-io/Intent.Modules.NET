using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Redis.Om.Repositories.Application.Clients.GetClientsByName
{
    public class GetClientsByNameQuery : IRequest<List<ClientDto>>, IQuery
    {
        public GetClientsByNameQuery(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}