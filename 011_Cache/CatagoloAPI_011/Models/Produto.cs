using CatagoloAPI.Validations;
using System.ComponentModel.DataAnnotations; // usando para definir as propriedades como obrigatórias, chave primária, etc.
using System.ComponentModel.DataAnnotations.Schema; // usando para mapear a classe para a tabela no banco de dados
using System.Text.Json.Serialization;

// propriedades as quais serão mapeadas para o banco de dados
namespace CatagoloAPI.Models;

[Table("Produtos")] // mapeando a classe Produto para a tabela Produtos no banco de dados
// 2ª abordagem: implementando a interface IValidatableObject
public class Produto : IValidatableObject
{
    [Key] // atributo que indica que a propriedade é a chave primária da tabela
    public int ProdutoId { get; set; }
    // Chave primária da categoria (é necessário que tenha 'Id' no nome – seja isolado ou junto com a classe em questão)

    [Required(ErrorMessage = "O nome é obrigatório")] // atributo que indica que a propriedade é obrigatória + uma mensagem personalizada
    [StringLength(80 , MinimumLength = 2 , ErrorMessage = "O nome deve ter entre 2 a 80 caracteres")] // atributo que define o tamanho máximo da string + tamanho mínimo + mensagem personalizada
    // [PrimeiraLetraMaiuscula] atributo personalizado que valida se a primeira letra é maiúscula
    public string? ProdutoNome { get; set; }

    [Required] // atributo que indica que a propriedade é obrigatória
    [StringLength(300 , ErrorMessage = "A descrição deve ter no máximo {1} caracteres")] // atributo que define o tamanho máximo da string + mensagem personalizada
    public string? ProdutoDescricao { get; set; }

    [Required] // atributo que indica que a propriedade é obrigatória
    [Column(TypeName = "decimal(10,2)")] // atributo que define o tipo da coluna no banco de dados como decimal com 10 dígitos no total e duas casas decimais
    [Range(1 , 10000 , ErrorMessage = "O preço deve estar entre {1} e {2}")] // atributo que define o intervalo de valores permitidos + mensagem personalizada
    public decimal ProdutoPreco { get; set; }

    [Required] // atributo que indica que a propriedade é obrigatória
    [StringLength(300)] // atributo que define o tamanho máximo da string
    public string? ProdutoImagemUrl { get; set; }

    public float ProdutoEstoque { get; set; }
    public DateTime DataCadastro { get; set; }

    // Chave estrangeira para a categoria: um produto pertence a uma categoria, então a relação é de N:1 (onde N = produtos; 1 = categoria)
    public int CategoriaId { get; set; }

    [JsonIgnore] // atributo que indica que a propriedade não deve ser serializada para JSON
    public Categoria? Categoria { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if(!string.IsNullOrEmpty(ProdutoNome))
        {
            var primeiraLetra = ProdutoNome[0].ToString();
            if(primeiraLetra != primeiraLetra.ToUpper())
            {
                // yield é um iterador que permite retornar cada elemento individualmente
                yield return new ValidationResult("A primeira letra do nome do produto deve ser maiúscula." , new[] { nameof(ProdutoNome) });
            }
        }

        if(ProdutoEstoque <= 0) yield return new ValidationResult("O estoque deve ser maior que zero." , new[] { nameof(ProdutoEstoque) });
    }
}
