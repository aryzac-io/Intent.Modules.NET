using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.FinbuckleMessageHeaderStrategy", Version = "1.0")]

namespace MassTransitFinbuckle.Test.Infrastructure.Eventing
{
    public class FinbuckleMessageHeaderStrategy : IMultiTenantStrategy
    {
        private string _tenantIdentifier = null!;

        public Task<string> GetIdentifierAsync(object context)
        {
            return Task.FromResult(_tenantIdentifier);
        }

        public void SetTenantIdentifier(string tenantIdentifier)
        {
            _tenantIdentifier = tenantIdentifier;
        }
    }
}