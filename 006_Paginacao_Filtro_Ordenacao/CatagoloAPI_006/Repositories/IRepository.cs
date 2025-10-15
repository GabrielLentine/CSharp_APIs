using System.Linq.Expressions;

namespace CatagoloAPI.Repositories;

public interface IRepository<T>
{
    IEnumerable<T> GetAll();

    T? GetById(Expression<Func<T, bool>> predicate);

    // T = Categoria ou Produto | bool = valor de retorno do predicate | predicate = expressão lambda que iremos verificar
    // Ex.: _repo.Get(c => c.CategoriaId == id); -> "c => c.CategoriaId == id" é o predicate; a resposta será um 'true' ou 'false'
    T Create(T item);
    T Update(T item);
    T Delete(T item);
}