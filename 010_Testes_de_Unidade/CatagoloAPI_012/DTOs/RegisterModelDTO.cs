using System.ComponentModel.DataAnnotations;

namespace CatagoloAPI.DTOs;
public class RegisterModelDTO
{
    [Required(ErrorMessage = "Nome de usuário é obrigatório!")]
    public string? Username { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "Email é obrigatório!")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Nome de usuário é obrigatório!")]
    public string? Password { get; set; }
}
