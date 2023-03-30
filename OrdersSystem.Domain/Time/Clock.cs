namespace OrdersSystem.Domain.Time
{
    public class Clock : IClock
    {
        public DateTime Now => DateTime.Now;
    }
}
