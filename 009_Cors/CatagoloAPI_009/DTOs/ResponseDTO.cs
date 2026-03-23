namespace CatagoloAPI.DTOs;

public class ResponseDTO
{
    public string? Status { get; set; } // Status da resposta (ex: "Success", "Error")
    public string? Message { get; set; } // Mensagem adicional, se necessário
}