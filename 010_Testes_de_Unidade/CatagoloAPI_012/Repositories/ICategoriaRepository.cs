using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using X.PagedList;

namespace CatagoloAPI.Repositories;

// a interface tem a vantagem de ter uma implementação flexível: eu posso ter várias implementações dentro desse repositório
public interface ICategoriaRepository : IRepository<Categoria>
{
    // Não vou mais precisar de nenhum desses contratos
    // IEnumerable<Categoria> GetCategorias();
    // Categoria GetCategoria(int id);
    // Categoria Create(Categoria categoria);
    // Categoria Update(Categoria categoria);
    // Categoria Delete(int id);

    Task<IPagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParameters);
    Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriasFiltroNome categoriasFiltroNome);
}