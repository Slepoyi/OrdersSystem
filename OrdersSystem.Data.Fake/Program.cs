using System.Text.Json;

//DataGenerator.InitData();

//foreach (var u in DataGenerator.Users) Print(u);
//Console.WriteLine("_________________________________________________________________");
//foreach (var c in DataGenerator.Customers) Print(c);
//Console.WriteLine("_________________________________________________________________");
//foreach (var s in DataGenerator.Skus) Print(s);
//Console.WriteLine("_________________________________________________________________");
//foreach (var op in DataGenerator.OrderPickers) Print(op);
//Console.WriteLine("_________________________________________________________________");
//foreach (var o in DataGenerator.Orders) Print(o);
//Console.WriteLine("_________________________________________________________________");
//foreach (var si in DataGenerator.StockItems) Print(si);

void Print<T>(T entity)
{
    var serialized = JsonSerializer.Serialize(entity);
    Console.WriteLine(serialized);
}