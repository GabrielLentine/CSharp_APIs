namespace CatagoloAPI.Repositories;

public interface IUnityOfWork
{
    /*
        IRepository<Produto> Produtos { get; }
        IRepository<Categoria> Categorias { get; }

        Quando o repositório genérico é usado, eu perco o uso dos métodos personalizados (em IProdutoRepository tem um método diferente, criado p/
        uso exclusivo dessa Interface), porém eu posso usar qualquer repositório que implemente a interface (como IRepository<Cliente>, por exemplo).
        Vale o exemplo p/ ver qual o melhor caminho.
    */

    IProdutoRepository ProdutoRepository { get; }
    ICategoriaRepository CategoriaRepository { get; }
    void Commit();
    void Dispose();
}