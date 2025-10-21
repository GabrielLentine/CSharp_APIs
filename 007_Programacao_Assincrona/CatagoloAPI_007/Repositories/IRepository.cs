using System.Linq.Expressions;

namespace CatagoloAPI.Repositories;

public interface IRepository<T>
{
    // a implementação do contrato p/ retornar dados usando GetAll e GetById precisa acessar o banco de dados
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Expression<Func<T, bool>> predicate);
    // T = Categoria ou Produto | bool = valor de retorno do predicate | predicate = expressão lambda que iremos verificar
    // Ex.: _repo.Get(c => c.CategoriaId == id); -> "c => c.CategoriaId == id" é o predicate; a resposta será um 'true' ou 'false'

    // os contratos de Create, Update e Delete não acessam o banco de dados, apenas realizam operações na memória
    T Create(T item);
    T Update(T item);
    T Delete(T item);
}