using CatagoloAPI.Context;
using CatagoloAPI.Models;

namespace CatagoloAPI.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
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