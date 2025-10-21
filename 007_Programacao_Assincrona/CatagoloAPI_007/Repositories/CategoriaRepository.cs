using CatagoloAPI.Context;
using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using X.PagedList;

namespace CatagoloAPI.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IPagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParameters)
    {
        // var categorias = GetAllAsync().OrderBy(c => c.CategoriaId).AsQueryable();
        var categorias = await GetAllAsync();
        var categoriasOrdenadas = categorias.OrderBy(c => c.CategoriaId).AsQueryable();
        var resultado = await categoriasOrdenadas.ToPagedListAsync(categoriasParameters.PageNumber, categoriasParameters.PageSize);

        return resultado;
    }

    public async Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriasFiltroNome categoriasFiltroNome)
    {
        var categorias = await GetAllAsync();
        if(!string.IsNullOrEmpty(categoriasFiltroNome.Nome))
        {
            categorias = categorias.Where(c => c.CategoriaNome.Contains(categoriasFiltroNome.Nome));
        }

        // var categoriasFiltradas = IPagedList<Categoria>.ToPagedList(categoria.AsQueryable(), categoriasFiltroNome.PageNumber, categoriasFiltroNome.PageSize);
        var categoriasFiltradas = await categorias.ToPagedListAsync(categoriasFiltroNome.PageNumber , categoriasFiltroNome.PageSize);

        return categoriasFiltradas;
    }

    /*public IEnumerable<Categoria> GetCategorias() => _context.Categorias.ToList();

    public Categoria GetCategoria(int id) => _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);

    public Categoria Create(Categoria categoria)
    {
        if(categoria == null) throw new ArgumentNullException(nameof(categoria));

        _context.Categorias.Add(categoria);
        _context.SaveChanges();

        return categoria;
    }
    public Categoria Update(Categoria categoria)
    {
        if(categoria == null) throw new ArgumentNullException(nameof(categoria));

        _context.Entry(categoria).State = EntityState.Modified;
        _context.SaveChanges();

        return categoria;
    }

    public Categoria Delete(int id)
    {
        var categoria = _context.Categorias.Find(id);

        if(categoria == null) throw new ArgumentNullException(nameof(categoria));

        _context.Categorias.Remove(categoria);
        _context.SaveChanges();

        return categoria;
    }*/
}