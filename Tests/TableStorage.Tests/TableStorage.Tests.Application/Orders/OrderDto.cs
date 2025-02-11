using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Application.Common.Mappings;
using TableStorage.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TableStorage.Tests.Application.Orders
{
    public class OrderDto : IMapFrom<Order>
    {
        public OrderDto()
        {
            PartitionKey = null!;
            RowKey = null!;
            OrderNo = null!;
            Customer = null!;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string OrderNo { get; set; }
        public decimal Amount { get; set; }
        public OrderCustomerDto Customer { get; set; }

        public static OrderDto Create(
            string partitionKey,
            string rowKey,
            string orderNo,
            decimal amount,
            OrderCustomerDto customer)
        {
            return new OrderDto
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                OrderNo = orderNo,
                Amount = amount,
                Customer = customer
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Order, OrderDto>();
        }
    }
}