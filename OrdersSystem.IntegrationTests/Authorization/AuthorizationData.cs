using OrdersSystem.Domain.Models.Auth;

namespace OrdersSystem.IntegrationTests.Authorization
{
    public class AuthorizationData
    {
        public static IEnumerable<object[]> CorrectExistingData =>
        new List<object[]>
        {
            new object[] { new LoginModel { Username = "", Password = "" } },
        };

        public static IEnumerable<object[]> IncorrectData =>
            new List<object[]>
            {
                new object[] { new LoginModel { Username = "", Password = "" } },
                new object[] { new LoginModel { Username = "Uuuu", Password = "1235123123123" } },
                new object[] { new LoginModel { Username = "Uuuu" } }
            };

        public static IEnumerable<object[]> UnexistingData =>
            new List<object[]>
            {
                new object[] { new LoginModel { Username = "MyLogin", Password = "12345678" } },
            };
    }
}
