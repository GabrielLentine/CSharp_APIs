using CatagoloAPI.Context;
using CatagoloAPI.Models;
using CatagoloAPI.Pagination;

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

    public PagedList<Produto> GetProdutos(ProdutosParameters produtosParameters)
    {
        var produtos = GetAll().OrderBy(p => p.ProdutoId).AsQueryable();
        var produtosOrdenados = PagedList<Produto>.ToPagedList(produtos, produtosParameters.PageNumber, produtosParameters.PageSize);

        return produtosOrdenados;
    }

    public PagedList<Produto> GetProdutosFiltroPreco(ProdutosFiltroPreco produtosFiltroPreco)
    {
        var produtos = GetAll().AsQueryable();

        if(produtosFiltroPreco.Preco.HasValue && !string.IsNullOrEmpty(produtosFiltroPreco.PrecoCriterio))
        {
            if(produtosFiltroPreco.PrecoCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
            {
                produtos = produtos.Where(p => p.ProdutoPreco > produtosFiltroPreco.Preco.Value).OrderBy(p => p.ProdutoPreco);
            }
            else if(produtosFiltroPreco.PrecoCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
            {
                produtos = produtos.Where(p => p.ProdutoPreco < produtosFiltroPreco.Preco.Value).OrderBy(p => p.ProdutoPreco);
            }
            else if(produtosFiltroPreco.PrecoCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
            {
                produtos = produtos.Where(p => p.ProdutoPreco == produtosFiltroPreco.Preco.Value).OrderBy(p => p.ProdutoPreco);
            }
        }

        var produtosFiltrados = PagedList<Produto>.ToPagedList(produtos , produtosFiltroPreco.PageNumber , produtosFiltroPreco.PageSize);
        return produtosFiltrados;
    }

    //public IEnumerable<Produto> GetProdutos(ProdutosParameters produtosParameters)
    //{
    //    // .Skip() -> calcular quantos itens devem ser pulados na consulta de dados
    //    // .Take() -> especificar quantos itens devem ser retornados na consulta de dados
    //    return GetAll()
    //        .OrderBy(p => p.ProdutoId)
    //        .Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
    //        .Take(produtosParameters.PageSize).ToList();
    //}

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