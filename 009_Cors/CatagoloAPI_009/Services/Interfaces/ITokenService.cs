using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CatagoloAPI.Services.Interfaces;
public interface ITokenService
{
    JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration _config);
    // esse método vai receber uma lista de claims (informações do usuário) e a configuração da aplicação (IConfiguration),
    // aí vai retornar um token JWT (JwtSecurityToken) que representa o token de acesso gerado com base nas claims fornecidas.

    string GenerateRefreshToken();
    // gerar o refresh token

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token, IConfiguration _config);
    // esse método vai extrair as informações das claims de um token expirado pra poder gerar um novo token usando o refresh token
}
