namespace OrdersSystem.Domain.Time
{
    public interface IClock
    {
        DateTime Now { get; }
    }
}
