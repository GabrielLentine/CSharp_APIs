using System.ComponentModel.DataAnnotations;

namespace CatagoloAPI.Validations;

public class RequiredCategoriaIdAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value , ValidationContext validationContext)
    {
        if(value == null) return new ValidationResult("O ID da categoria é obrigatório!");

        if(value is int categoriaId && categoriaId <= 0) return new ValidationResult("É necessário informar o ID para colocar o produto na categoria correta!");

        return ValidationResult.Success;
    }
}