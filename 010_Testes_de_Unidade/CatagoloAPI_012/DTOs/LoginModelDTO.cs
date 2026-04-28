using System.ComponentModel.DataAnnotations;

namespace CatagoloAPI.DTOs;
public class LoginModelDTO
{
    [Required(ErrorMessage = "Nome de usuário é obrigatório!")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Nome de usuário é obrigatório!")]
    public string? Password { get; set; }
}
