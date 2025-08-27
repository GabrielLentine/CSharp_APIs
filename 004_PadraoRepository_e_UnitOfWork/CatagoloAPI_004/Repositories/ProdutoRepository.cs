using CatagoloAPI.Context;
using CatagoloAPI.Models;

namespace CatagoloAPI.Repositories;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context)
    {
    }

    public IEnumerable<Produto> GetProdutosPorCategoria(int id)
    {
        return GetAll().Where(c => c.CategoriaId == id);
    }

    /*
    representa a seleção de todos os produtos da tabela Produtos no banco de dados
    public IQueryable<Produto> GetProdutos()
    {
        // agora posso realizar a paginação dentro do banco de dados p/ consultas específicas
        return _context.Produtos;
    }

    public Produto GetProdutoId(int id)
    {
        var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

        if (produto == null) throw new InvalidOperationException($"Produto {id} não encontrado");

        return produto;
    }

    public Produto CreateProduto(Produto produto)
    {
        if (produto == null) throw new ArgumentException(nameof(produto));

        _context.Produtos.Add(produto);
        _context.SaveChanges();

        return produto;
    }

    public bool UpdateProduto(Produto produto)
    {
        if (produto == null) throw new ArgumentException(nameof(produto));

        // 'Any' verifica se algum elemento na coleção atende a determinada condição
        if (_context.Produtos.Any(p => p.ProdutoId == produto.ProdutoId))
        {
            _context.Produtos.Update(produto);
            _context.SaveChanges();
            return true;
        }

        return false;
    }

    public bool DeleteProduto(int id)
    {
        var produtoDeletado = _context.Produtos.Find(id);

        if (produtoDeletado != null)
        {
            _context.Produtos.Remove(produtoDeletado);
            _context.SaveChanges();
            return true;
        }

        return false;
    }*/
}