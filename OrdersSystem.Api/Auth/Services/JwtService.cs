using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrdersSystem.Api.Options;
using OrdersSystem.Domain.Models.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OrdersSystem.Api.Auth.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtOptions _jwtOptions;
        public JwtService(IOptions<JwtOptions> options)
        {
            _jwtOptions = options.Value;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(_jwtOptions.Secret);
            //var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim("id", user.Id.ToString()),
                        new Claim("role", user.Role)
                    }),
                Audience = _jwtOptions.ValidAudience,
                Issuer = _jwtOptions.ValidIssuer,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public Guid? ValidateTokenAndExtractId(string? token)
        {
            if (token is null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(_jwtOptions.Secret);
            //var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ClockSkew = _jwtOptions.ClockSkew,
                    ValidateIssuer = _jwtOptions.ValidateIssuer,
                    ValidateAudience = _jwtOptions.ValidateAudience,
                    ValidateLifetime = _jwtOptions.ValidateLifetime,
                    ValidateIssuerSigningKey = _jwtOptions.ValidateIssuerSigningKey,
                    ValidAudience = _jwtOptions.ValidAudience,
                    ValidIssuer = _jwtOptions.ValidIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                return Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
            }
            catch
            {
                return null;
            }
        }
    }
}
