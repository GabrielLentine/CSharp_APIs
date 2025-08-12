using CatagoloAPI.Context;
using CatagoloAPI.Filters;
using CatagoloAPI.Models;
using CatagoloAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatagoloAPI.Controllers;

[Route("[controller]")] // define a rota base para este controlador (usando o nome do controlador -> '/categorias')
[ApiController] // indica que este controlador é um controlador de API
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    public CategoriasController(AppDbContext context , IConfiguration configuration , ILogger<CategoriasController> logger)
    {
        // injeta o contexto do banco de dados no controlador
        _context = context;

        // injeta a configuração do aplicativo no controlador
        _configuration = configuration;

        // injeta o logger no controlador
        _logger = logger;
    }

    [HttpGet("LerArquivoConfiguracao")]
    public string GetValores()
    {
        var valor1 = _configuration["chave1"];
        var valor2 = _configuration["chave2"];

        var secao1 = _configuration["secao1:chave2"];

        return $"Chave 1 = {valor1} \nChave 2 = {valor2} \nSeção 1 => Chave 2 = {secao1}";
    }

    // GET FromServices antes do .NET 7.0
    [HttpGet("UsandoFromServices/{nome}")]
    public ActionResult<string> GetSaudacaoFromServices([FromServices] IMeuServico servico , string nome)
    {
        return servico.Saudacao(nome);
    }

    // GET FromServices após o .NET 7.0
    [HttpGet("SemUsarFromServices/{nome}")]
    public ActionResult<string> GetSaudacaoSemFromServices(IMeuServico servico , string nome)
    {
        // a partir do .NET 7.0, não é mais necessário usar o [FromServices] para injetar serviços no método
        return servico.Saudacao(nome);
    }

    [HttpGet] // define o método HTTP GET para este endpoint
    [ServiceFilter(typeof(ApiLoggingFilter))]
    public ActionResult<IEnumerable<Categoria>> Get()
    {
        try
        {
            _logger.LogInformation("====== GET categorias ======");

            // busca todas as categorias do banco de dados sem rastreamento (AsNoTracking) para melhorar a performance
            // só posso usar o AsNoTracking quando não for necessário rastrear as entidades retornadas (como em consultas de leitura)
            var categorias = _context.Categorias.AsNoTracking().ToList();
            if(categorias == null) return NotFound();
            return categorias;
        }
        catch(Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError , "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    [HttpGet("produtos")] // define o método HTTP GET para este endpoint com a rota 'categorias/produtos'
    public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
    {
        _logger.LogInformation("====== GET categorias/produtos ======");

        // return _context.Categorias.Include(p => p.Produtos).AsNoTracking().ToList();
        return _context.Categorias.Include(p => p.Produtos).Where(c => c.CategoriaId <= 5).AsNoTracking().ToList();
        // retorna as categorias com os produtos relacionados, limitando até o ID 5
    }

    // GET ID
    [HttpGet("{id:int}" , Name = "ObterCategoria")] // define o método HTTP GET para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    // o Name define um nome para a rota, que pode ser usado para referenciar esta rota em outros lugares (como no método Post)
    public ActionResult<Categoria> GetId(int id)
    {
        // throw new Exception("Exceção ao retornar o produto pelo ID");

        var categoria = _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);

        _logger.LogInformation($"====== GET categorias/produtos/id{id} ======");

        if(categoria == null)
        {
            _logger.LogInformation($"====== GET categorias/produtos/id{id} Not Found ======");
            return NotFound(); // 404 Not Found se não houver a categoria
        }
        return Ok(categoria); // 200 OK com a categoria encontrada
    }

    // POST
    [HttpPost] // define o método HTTP POST para este endpoint
    public ActionResult Post(Categoria c)
    {
        if(c == null) return BadRequest(); // 400 Bad Request se não houver a categoria
        _context.Categorias.Add(c); // adiciona a categoria ao contexto do banco de dados
        _context.SaveChanges(); // salva as alterações no banco de dados
        return new CreatedAtRouteResult("ObterCategoria" , new { id = c.CategoriaId } , c);
        // retorna uma resposta 201 Created com a rota para a categoria recém-criada
    }

    // PUT
    [HttpPut("{id:int}")] // define o método HTTP PUT para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    public ActionResult Put(int id , Categoria c)
    {
        if(id != c.CategoriaId) return BadRequest();
        _context.Entry(c).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        // marca a categoria como modificada no contexto do banco de dados

        _context.SaveChanges(); // salva as alterações no banco de dados
        return Ok(c);
    }

    // DELETE
    [HttpDelete("{id:int}")] // define o método HTTP DELETE para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    public ActionResult Delete(int id)
    {
        var categoria = _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);
        if(categoria == null) return NotFound("Categoria não localizada!"); // 404 Not Found se não houver a categoria (com uma mensagem)
        _context.Categorias.Remove(categoria);
        _context.SaveChanges();
        return Ok(categoria);
    }
}
