using System.Security.Cryptography;
using System.Text;

namespace OrdersSystem.Domain.Helper
{
    public class Md5HashProvider : ICryptoProvider
    {
        public string CreateCryptoString(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = MD5.HashData(inputBytes);

            var sb = new StringBuilder();
            for (var i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
