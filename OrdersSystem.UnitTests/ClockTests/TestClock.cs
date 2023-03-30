using OrdersSystem.Domain.Time;

namespace OrdersSystem.UnitTests.ClockTests
{
    internal class TestClock : IClock
    {
        private readonly DateTime _fixedDateTime;

        public TestClock(DateTime dateTime)
        {
            _fixedDateTime = dateTime;
        }

        public DateTime Now => _fixedDateTime;
    }
}
