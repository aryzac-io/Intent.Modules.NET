using System;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Identity.AccountController.AccountEmailSender", Version = "1.0")]

namespace Application.Identity.AccountController.Application.Account;

public class AccountEmailSender : IAccountEmailSender
{
    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public async Task SendEmailConfirmationRequest(string email, string userId, string code)
    {
        throw new NotImplementedException();
    }
}