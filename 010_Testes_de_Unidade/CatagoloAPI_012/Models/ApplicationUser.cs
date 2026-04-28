using Microsoft.AspNetCore.Identity;

namespace CatagoloAPI.Models;
public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; } // Token de atualização (refresh token) para renovar o token de acesso
    public DateTime? RefreshTokenExpiryTime { get; set; } // Data e hora de expiração do token de atualização
}
