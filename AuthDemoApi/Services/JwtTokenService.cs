using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthDemoApi.Services
{
    public class JwtTokenService
    {
        private const string JwtSecretKey = "EzEgyNagyonHosszuEsBiztonsagosTitkosKulcs";
        private const string Issuer = "TananyagApi";
        private const string Audience = "TananyagKliens";

        public string GenerateToken(string userId, string username, string role)
        {
            // 1. Payload (Claims) összeállítása
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role) // RBAC
        };

            // 2. Aláíró kulcs és algoritmus (Header + Signature)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. JWT példányosítása
            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
