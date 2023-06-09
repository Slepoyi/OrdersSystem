﻿using OrdersSystem.Domain.Models.Ordering;

namespace OrdersSystem.IntegrationTests.Tests.Customer
{
    public class CustomerData
    {
        public static IEnumerable<object[]> CorrectOrderItems =>
            new List<object[]>
            {
                new object[]
                {
                    new List<OrderItem>
                    {
                        new OrderItem
                        {
                            SkuId = new Guid("B9062C62-9A5D-A0FB-CDBA-EB80445E1187"),
                            Quantity = 6
                        },
                        new OrderItem
                        {
                            SkuId = new Guid("613DCD97-383E-ADD6-4E28-337396AD9585"),
                            Quantity = 4
                        },
                        new OrderItem
                        {
                            SkuId = new Guid("833C33B0-35A1-84B3-01B6-68F725707101"),
                            Quantity = 2
                        }
                    }
                }
            };

        // duplicate skuid, zero quantity and more than on stock
        public static IEnumerable<object[]> IncorrectOrderItems =>
           new List<object[]>
           {
                new object[]
                {
                    new List<OrderItem>
                    {
                        new OrderItem
                        {
                            SkuId = new Guid("B9062C62-9A5D-A0FB-CDBA-EB80445E1187"),
                            Quantity = 10000
                        },
                        new OrderItem
                        {
                            SkuId = new Guid("613DCD97-383E-ADD6-4E28-337396AD9585"),
                            Quantity = 0
                        },
                        new OrderItem
                        {
                            SkuId = new Guid("B9062C62-9A5D-A0FB-CDBA-EB80445E1187"),
                            Quantity = 8
                        }
                    }
                }
           };
    }
}
