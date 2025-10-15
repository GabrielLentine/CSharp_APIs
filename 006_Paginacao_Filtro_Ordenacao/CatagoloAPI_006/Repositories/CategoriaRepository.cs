using CatagoloAPI.Context;
using CatagoloAPI.Models;
using CatagoloAPI.Pagination;

namespace CatagoloAPI.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }

    public PagedList<Categoria> GetCategorias(CategoriasParameters categoriasParameters)
    {
        var categorias = GetAll().OrderBy(c => c.CategoriaId).AsQueryable();
        var categoriasOrdenadas = PagedList<Categoria>.ToPagedList(categorias, categoriasParameters.PageNumber, categoriasParameters.PageSize);

        return categoriasOrdenadas;
    }

    public PagedList<Categoria> GetCategoriasFiltroNome(CategoriasFiltroNome categoriasFiltroNome)
    {
        var categoria = GetAll().AsQueryable();
        if(!string.IsNullOrEmpty(categoriasFiltroNome.Nome))
        {
            categoria = categoria.Where(c => c.CategoriaNome.Contains(categoriasFiltroNome.Nome));
        }
        var categoriasFiltradas = PagedList<Categoria>.ToPagedList(categoria, categoriasFiltroNome.PageNumber, categoriasFiltroNome.PageSize);
        
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