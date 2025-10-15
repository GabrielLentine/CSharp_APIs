using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

// usando para definir as propriedades como obrigatórias, chave primária, etc.

// usando para mapear a classe para a tabela no banco de dados

// propriedades as quais serão mapeadas para o banco de dados
namespace CatagoloAPI.Models;

[Table("Categorias")] // mapeando a classe Categoria para a tabela Categorias no banco de dados
public class Categoria
{
    public Categoria()
    {
        // Inicializa a coleção de produtos (boa prática -> responsabilidade da classe onde é definido a propriedade do tipo Collection)
        Produtos = new Collection<Produto>();
    }

    [Key] public int CategoriaId { get; set; }
    // Chave primária da categoria (é necessário que tenha 'Id' no nome – seja isolado ou junto com a classe em questão)

    [Required] // atributo que indica que a propriedade é obrigatória
    [StringLength(80)] // atributo que define o tamanho máximo da string
    public string? CategoriaNome { get; set; }

    [Required] // atributo que indica que a propriedade é obrigatória
    [StringLength(300)] // atributo que define o tamanho máximo da string
    public string? CategoriaImagemUrl { get; set; }

    // uma categoria pode ter vários produtos, então a relação é de 1:N (onde 1 = categoria; N = produtos)
    [JsonIgnore] public ICollection<Produto>? Produtos { get; set; }
}