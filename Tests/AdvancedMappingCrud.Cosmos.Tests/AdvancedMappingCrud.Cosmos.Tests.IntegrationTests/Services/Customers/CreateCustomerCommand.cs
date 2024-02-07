using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Customers
{
    public class CreateCustomerCommand
    {
        public CreateCustomerCommand()
        {
            Name = null!;
            Surname = null!;
            Line1 = null!;
            Line2 = null!;
            City = null!;
            PostalCode = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public static CreateCustomerCommand Create(
            string name,
            string surname,
            bool isActive,
            string line1,
            string line2,
            string city,
            string postalCode)
        {
            return new CreateCustomerCommand
            {
                Name = name,
                Surname = surname,
                IsActive = isActive,
                Line1 = line1,
                Line2 = line2,
                City = city,
                PostalCode = postalCode
            };
        }
    }
}