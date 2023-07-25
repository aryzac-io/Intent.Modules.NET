using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Repositories.DDD;
using CleanArchitecture.TestApplication.Domain.Services.DDD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.DDD.AccountTransfer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AccountTransferHandler : IRequestHandler<AccountTransfer>
    {
        private readonly IAccountHolderRepository _accountHolderRepository;
        private readonly IAccountingDomainService _domainService;

        [IntentManaged(Mode.Merge)]
        public AccountTransferHandler(IAccountHolderRepository accountHolderRepository,
            IAccountingDomainService domainService)
        {
            _accountHolderRepository = accountHolderRepository;
            _domainService = domainService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(AccountTransfer request, CancellationToken cancellationToken)
        {
            var entity = await _accountHolderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entity is null)
            {
                throw new NotFoundException($"Could not find AccountHolder '{request.Id}'");
            }

            entity.Transfer(request.Description, _domainService, request.Amount, request.Currency);

        }
    }
}