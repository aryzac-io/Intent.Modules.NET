using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Customers.DeleteCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Ignore)]
        public DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var existingCustomer = await _customerRepository.FindByIdAsync(request.Id, cancellationToken);
            _customerRepository.Remove(existingCustomer);
            return Unit.Value;
        }
    }
}