using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Orders
{
    public class CreateOrderCommand
    {
        public CreateOrderCommand()
        {
            CustomerId = null!;
            RefNo = null!;
            OrderTags = null!;
            OrderItems = null!;
        }

        public string CustomerId { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public List<CreateOrderCommandOrderTagsDto> OrderTags { get; set; }
        public List<CreateOrderCommandOrderItemsDto> OrderItems { get; set; }

        public static CreateOrderCommand Create(
            string customerId,
            string refNo,
            DateTime orderDate,
            OrderStatus orderStatus,
            List<CreateOrderCommandOrderTagsDto> orderTags,
            List<CreateOrderCommandOrderItemsDto> orderItems)
        {
            return new CreateOrderCommand
            {
                CustomerId = customerId,
                RefNo = refNo,
                OrderDate = orderDate,
                OrderStatus = orderStatus,
                OrderTags = orderTags,
                OrderItems = orderItems
            };
        }
    }
}