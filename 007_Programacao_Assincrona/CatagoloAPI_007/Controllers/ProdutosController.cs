using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using CatagoloAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CatagoloAPI.DTOs;
using Newtonsoft.Json;
using X.PagedList;

namespace CatagoloAPI.Controllers;

[Route("[controller]")] // define a rota base para este controlador (usando o nome do controlador -> '/produtos')
[ApiController] // indica que este controlador é um controlador de API
public class ProdutosController : ControllerBase
{
    // private readonly IProdutoRepository _produtoRepository;
    // private readonly IRepository<Produto> _repository;
    private readonly IUnityOfWork _uof;
    private readonly IMapper _mapper;

    public ProdutosController(IUnityOfWork uof, IMapper mapper)
    {
        _uof = uof;
        _mapper = mapper;
    }

    [HttpGet("categorias/{id}")]
    public async Task<ActionResult<IEnumerable<Produto>>> GetProdutosCategoria(int id)
    {
        var produtos = await _uof.ProdutoRepository.GetProdutosPorCategoriaAsync(id);

        if (produtos == null) return NotFound();

        return Ok(produtos);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetPagination([FromQuery] ProdutosParameters produtosParameters)
    {
        var produtos = await _uof.ProdutoRepository.GetProdutosAsync(produtosParameters);
        return ObterProdutos(produtos);
    }

    [HttpGet("filtro/preco/pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosFilterPreco([FromQuery] ProdutosFiltroPreco produtosFiltroPreco)
    {
        var produtos = await _uof.ProdutoRepository.GetProdutosFiltroPrecoAsync(produtosFiltroPreco);
        return ObterProdutos(produtos);
    }

    private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutos(IPagedList<Produto> produtos)
    {
        var metadata = new
        {
            produtos.Count ,
            produtos.PageSize ,
            produtos.PageCount ,
            produtos.TotalItemCount ,
            produtos.HasNextPage ,
            produtos.HasPreviousPage
        };
        Response.Headers.Append("X-Pagination" , JsonConvert.SerializeObject(metadata));

        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return Ok(produtosDto);
    }

    // GET -> /produtos
    [HttpGet] // define o método HTTP GET para este endpoint
    // usando IEnumerable eu trabalharei por demanda, ou seja, não trago todos os produtos de uma vez, mas sim conforme a necessidade,
    // sendo assim mais otimizado
    public async Task<ActionResult<IEnumerable<Produto>>> Get()
    {
        // busca todos os produtos do banco de dados sem rastreamento (AsNoTracking) para melhorar a performance
        // só posso usar o AsNoTracking quando não for necessário rastrear as entidades retornadas (como em consultas de leitura)
        // var produtos = _produtoRepository.GetProdutos().OrderBy(p => p.ProdutoId).ToList();

        var produtos = await _uof.ProdutoRepository.GetAllAsync();
        if (produtos == null) return NotFound(); // 404 Not Found se não houver

        return Ok(produtos);
    }

    // GET ID -> /produtos/id
    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    // define o método HTTP GET para este endpoint com um parâmetro de rota 'id' do tipo inteiro e que o valor mínimo seja 1
    // o Name define um nome para a rota, que pode ser usado para referenciar esta rota em outros lugares (como no método Post)
    public async Task<ActionResult<Produto>> GetId(int id)
    {
        // busca um produto específico pelo ID do banco de dados de forma assíncrona
        var produto = await _uof.ProdutoRepository.GetByIdAsync(p => p.ProdutoId == id);
        if (produto == null) return NotFound();

        return Ok(produto);
    }

    // POST -> /produtos
    [HttpPost] // define o método HTTP POST para este endpoint
    // ActionResult é usado para retornar diferentes tipos de respostas HTTP
    public async Task<ActionResult> Post(Produto p)
    {
        if (p == null) return BadRequest(); // 400 Bad Request se não houver o produto

        var novoProduto = _uof.ProdutoRepository.Create(p);
        await _uof.CommitAsync();

        return new CreatedAtRouteResult("ObterProduto", new { id = novoProduto.ProdutoId }, novoProduto);
        // retorna uma resposta 201 Created com a rota para o produto recém-criado
    }

    // PUT -> /produtos/id
    [HttpPut("{id:int}")] // define o método HTTP PUT para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    public async Task<ActionResult> Put(int id, Produto p)
    {
        if (id != p.ProdutoId) return BadRequest(); // 400 Bad Request se o ID do produto não corresponder ao ID na rota

        var produtoAtualizado = _uof.ProdutoRepository.Update(p);
        await _uof.CommitAsync();

        return Ok(produtoAtualizado);
    }

    // DELETE -> /produtos/id
    [HttpDelete("{id:int}")] // define o método HTTP DELETE para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    public async Task<ActionResult> Delete(int id)
    {
        var produto = await _uof.ProdutoRepository.GetByIdAsync(p => p.ProdutoId == id);
        if (produto == null) return NotFound();

        var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
        await _uof.CommitAsync();

        return Ok(produtoDeletado);
    }

    // /produtos/primeiro
    // [HttpGet("/primeiro")] -> é possível definir mais de uma rota para o mesmo método
    // [HttpGet("{valor:alpha")] -> valor:alpha define que o parâmetro 'valor' deve ser uma string alfabética (A-Z, a-z)
    // [HttpGet("{valor:alpha:length(5)")] -> valor:alpha:length(5) define que o parâmetro deve ser uma string alfabética com exatamente 5 caracteres
    // [HttpGet("{valor:alpa:maxlength(5)")] -> valor:alpa:maxlength(5) define que o parâmetro deve ser uma string alfabética com no máximo 5 caracteres
    // [HttpGet("primeiro")]
    // public ActionResult<Produto> GetPrimeiro()
    // {
    //     // busca o primeiro produto do banco de dados
    //     var produto = _context.Produtos.AsNoTracking().FirstOrDefault();
    //     
    //     if (produto == null) return NotFound(); // 404 Not Found se não houver
    //
    //     return produto;
    // }

    // [HttpGet("async")]
    // public async Task<ActionResult<IEnumerable<Produto>>> GetAsync()
    // {
    //     return await _context.Produtos.AsNoTracking().ToListAsync();
    //     // busca todos os produtos do banco de dados de forma assíncrona
    // }
}