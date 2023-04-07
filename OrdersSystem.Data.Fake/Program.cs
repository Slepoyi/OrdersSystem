using Bogus;
using OrdersSystem.Domain.Models.Ordering;

Console.WriteLine("Hello, World!");

var customerFaker = new Faker<Customer>()
    .RuleFor(c => c.Id, Guid.NewGuid())
    .RuleFor(c => c.Name, (f, c) => f.Name.FullName())
    .RuleFor(c => c.Email, (f, c) => f.Internet.Email())
    .RuleFor(c => c.Address, (f, c) => f.Address.StreetAddress(false))
    .RuleFor(c => c.Phone, (f, c) => f.Phone.PhoneNumber("###-###-####")); // add orders
