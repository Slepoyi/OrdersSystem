using OrdersSystem.Domain.Models.Auth;

namespace OrdersSystem.IntegrationTests.Tests.Authorization
{
    public class AuthorizationData
    {
        public static IEnumerable<object[]> CorrectExistingData =>
            new List<object[]>
            {
                new object[] { new LoginModel { Username = "Aliya_Cruickshank", Password = "PVBCbyoW" } },
                new object[] { new LoginModel { Username = "Arthur_Bayer", Password = "xt33OZOb" } }
            };

        public static IEnumerable<object[]> CoorectUnexistingData =>
            new List<object[]>
            {
                new object[] { new LoginModel { Username = "AAAAAAAAA", Password = "xZbtseVy" } },
                new object[] { new LoginModel { Username = "MyLogin", Password = "12345678" } },
            };

        public static IEnumerable<object[]> IncorrectData =>
            new List<object[]>
            {
                new object[] { new LoginModel { Username = "", Password = "" } },
                new object[] { new LoginModel { Username = "D123", Password = "1235123123123" } },
                new object[] { new LoginModel { Username = "YesYes" } }
            };
    }
}
