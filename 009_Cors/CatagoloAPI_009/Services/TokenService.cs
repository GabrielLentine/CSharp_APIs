using CatagoloAPI.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CatagoloAPI.Services;

public class TokenService : ITokenService
{
    public JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims , IConfiguration _config)
    {
        // obtendo a chave secreta
        var key = _config.GetSection("JWT").GetValue<string>("SecretKey") ?? throw new InvalidOperationException("Chave secreta inválida");
        
        // convertendo a chave p/ um array de bytes
        var privateKey = Encoding.UTF8.GetBytes(key);

        // criando as credenciais p/ assinar o token
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(privateKey) , SecurityAlgorithms.HmacSha256Signature);

        // gerando a descrição das informações usadas p/ gerar o token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims) , // obtendo as informações do usuário
            Expires = DateTime.UtcNow.AddMinutes(_config.GetSection("JWT").GetValue<double>("TokenValidityInMinutes")) , // data de expiração
            Audience = _config.GetSection("JWT").GetValue<string>("ValidAudience"), // audiência
            Issuer = _config.GetSection("JWT").GetValue<string>("ValidIssuer"), // emissor
            SigningCredentials = signingCredentials // o token assinado acima será colocado aqui
        };

        // manipulador do token JWT - responsável por criar e validar os tokens
        var tokenHandler = new JwtSecurityTokenHandler();
        
        // criando e retornando o token
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

        return token;
    }

    public string GenerateRefreshToken()
    {
        // 128 bytes aleatórios gerados de forma segura
        var secureRandomBytes = new byte[128];

        // gerador de números aleatórios criptograficamente seguros
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(secureRandomBytes); // preenchendo o array com bytes aleatórios

        // convertendo os bytes aleatórios p/ uma string em Base64 e retornando o refresh token
        var refreshToken = Convert.ToBase64String(secureRandomBytes);
        return refreshToken;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token , IConfiguration _config)
    {
        // obtendo a chave secreta
        var secretKey = _config["JWT:SecretKey"] ?? throw new InvalidOperationException("Chave secreta inválida!");

        // configurando os parâmetros de validação do token
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false , // não validar audiência
            ValidateIssuer = false , // não validar emissor
            ValidateIssuerSigningKey = true , // validar a chave de assinatura do emissor
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)) , // configurando a assinatura do emissor usando a chave secreta
            ValidateLifetime = false // não validar o tempo de vida do token (pois ele pode estar expirado)
        };
        // manipulador do token JWT
        var tokenHandler = new JwtSecurityTokenHandler();

        // validando o token e obtendo as claims (informações do usuário)
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        // verificando se o token não é um uma instância jwtSecurityToken ou se o algoritmo de assinatura não é HMAC SHA256
        if(securityToken is not JwtSecurityToken jwtSecurityToken ||
           !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Token inválido!");
        }

        // retornando as claims do token expirado
        return principal;
    }
}
