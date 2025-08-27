using CatagoloAPI.Models;

namespace CatagoloAPI.Repositories;

public interface IProdutoRepository : IRepository<Produto>
{
    // Não vou mais precisar de nenhum desses contratos
    // IQueryable<Produto> GetProdutos();
    // Produto GetProdutoId(int id);
    // Produto CreateProduto(Produto produto);
    // bool UpdateProduto(Produto produto);
    // bool DeleteProduto(int id);

    IEnumerable<Produto> GetProdutosPorCategoria(int id);
}