namespace CatagoloAPI.DTOs;
public class TokenModelDTO
{
    public string? AccessToken { get; set; } // JWT de acesso
    public string? RefreshToken { get; set; } // Token de atualização
}
