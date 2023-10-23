using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjetoIBGE.Models;
using ProjetoIBGE.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjetoIBGE.Services
{
    public class TokenService
    {
        public string GenerateToken(User user)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            // em array de bytes
            var chave = Encoding.ASCII.GetBytes(Configuracao.Key);
    
            var token = jwtSecurityTokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(45),
                SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(chave),
            SecurityAlgorithms.HmacSha256Signature)
            });
            return jwtSecurityTokenHandler.WriteToken(token);
        }
    }
}
