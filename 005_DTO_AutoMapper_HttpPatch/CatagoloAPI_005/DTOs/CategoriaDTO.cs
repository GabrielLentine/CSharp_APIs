using System.ComponentModel.DataAnnotations;

namespace CatagoloAPI.DTOs;

public class CategoriaDTO
{
    public int CategoriaId { get; set; }

    [Required] [StringLength(80)] public string? CategoriaNome { get; set; }

    [Required] [StringLength(300)] public string? CategoriaImagemUrl { get; set; }
}