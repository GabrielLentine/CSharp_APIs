using System.ComponentModel.DataAnnotations;

namespace CatagoloAPI.Validations
{
    // 1ª abordagem: atributos personalizados
    public class PrimeiraLetraMaiusculaAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value , ValidationContext validationContext)
        {
            // já tem uma pré validação no model, então ele só passa
            if(value == null || string.IsNullOrWhiteSpace(value.ToString())) return ValidationResult.Success;

            var primeiraLetra = value.ToString()[0].ToString();
            if(primeiraLetra != primeiraLetra.ToUpper()) return new ValidationResult("A primeira letra deve ser maiúscula.");

            return ValidationResult.Success;
        }
    }
}
