using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using X.PagedList;

namespace CatagoloAPI.Repositories;

public interface IProdutoRepository : IRepository<Produto>
{
    // Não vou mais precisar de nenhum desses contratos
    // IQueryable<Produto> GetProdutos();
    // Produto GetProdutoId(int id);
    // Produto CreateProduto(Produto produto);
    // bool UpdateProduto(Produto produto);
    // bool DeleteProduto(int id);

    Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int id);
    // IEnumerable<Produto> GetProdutos(ProdutosParameters produtosParameters);
    Task<IPagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParameters);
    Task<IPagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroPreco);
}