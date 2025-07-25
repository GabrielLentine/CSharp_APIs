using System.ComponentModel.DataAnnotations; // usando para definir as propriedades como obrigatórias, chave primária, etc.
using System.ComponentModel.DataAnnotations.Schema; // usando para mapear a classe para a tabela no banco de dados
using System.Text.Json.Serialization;

// propriedades as quais serão mapeadas para o banco de dados
namespace CatagoloAPI.Models;

[Table("Produtos")] // mapeando a classe Produto para a tabela Produtos no banco de dados
public class Produto
{
    [Key] // atributo que indica que a propriedade é a chave primária da tabela
    public int ProdutoId { get; set; }
    // Chave primária da categoria (é necessário que tenha 'Id' no nome – seja isolado ou junto com a classe em questão)

    [Required] // atributo que indica que a propriedade é obrigatória
    [StringLength(80)] // atributo que define o tamanho máximo da string
    public string? ProdutoNome { get; set; }

    [Required] // atributo que indica que a propriedade é obrigatória
    [StringLength(300)] // atributo que define o tamanho máximo da string
    public string? ProdutoDescricao { get; set; }

    [Required] // atributo que indica que a propriedade é obrigatória
    [Column(TypeName = "decimal(10,2)")] // atributo que define o tipo da coluna no banco de dados como decimal com 10 dígitos no total e duas casas decimais
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
}
