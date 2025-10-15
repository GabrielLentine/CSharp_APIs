using System.Linq.Expressions;
using CatagoloAPI.Context;
using Microsoft.EntityFrameworkCore;

namespace CatagoloAPI.Repositories;

// "where T : class" significa que T deverá ser uma classe, ou seja, em qualquer lugar que esse repositório for chamado, o destino deve ser uma classe 
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<T> GetAll()
    {
        // 'Set<T>' é usado p/ acessar uma coleção ou tabela. Quando chamamos Set<T>, estamos chamando a coleção correspondendente
        return _context.Set<T>().AsNoTracking().ToList();
    }

    public T? GetById(Expression<Func<T, bool>> predicate)
    {
        // vai retornar a primeira entidade ou null
        return _context.Set<T>().FirstOrDefault(predicate);
    }

    public T Create(T item)
    {
        _context.Set<T>().Add(item);

        // _context.SaveChanges();
        return item;
    }

    public T Update(T item)
    {
        // _context.Set<T>().Update(item); -> também é possível usar
        _context.Entry(item).State = EntityState.Modified;
        // _context.SaveChanges();
        return item;
    }

    public T Delete(T item)
    {
        _context.Set<T>().Remove(item);
        // _context.SaveChanges();
        return item;
    }
}