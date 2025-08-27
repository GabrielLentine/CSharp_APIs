using CatagoloAPI.Models;
using CatagoloAPI.Repositories;
using CatagoloAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatagoloAPI.Controllers;

[Route("[controller]")] // define a rota base para este controlador (usando o nome do controlador -> '/categorias')
[ApiController] // indica que este controlador é um controlador de API
public class CategoriasController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    private readonly IUnityOfWork _uof;
    // private readonly IRepository<Categoria> _repository;
    // private readonly ICategoriaRepository _repository;

    public CategoriasController(IUnityOfWork uof, IConfiguration configuration,
        ILogger<CategoriasController> logger)
    {
        _uof = uof;

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
    public ActionResult<string> GetSaudacaoFromServices([FromServices] IMeuServico servico, string nome)
    {
        return servico.Saudacao(nome);
    }

    // GET FromServices após o .NET 7.0
    [HttpGet("SemUsarFromServices/{nome}")]
    public ActionResult<string> GetSaudacaoSemFromServices(IMeuServico servico, string nome)
    {
        // a partir do .NET 7.0, não é mais necessário usar o [FromServices] para injetar serviços no método
        return servico.Saudacao(nome);
    }

    [HttpGet] // define o método HTTP GET para este endpoint
    public ActionResult<IEnumerable<Categoria>> GetCategorias()
    {
        _logger.LogInformation("====== GET categorias ======");

        var categorias = _uof.CategoriaRepository.GetAll();

        if (categorias == null)
        {
            _logger.LogInformation("Erro 404: categoria não encontrada");
            return NotFound();
        }

        return Ok(categorias);
    }

    // GET ID
    [HttpGet("{id:int}",
        Name = "ObterCategoria")] // define o método HTTP GET para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    // o Name define um nome para a rota, que pode ser usado para referenciar esta rota em outros lugares (como no método Post)
    public ActionResult<Categoria> GetId(int id)
    {
        // throw new Exception("Exceção ao retornar o produto pelo ID");

        var categoria = _uof.CategoriaRepository.GetById(c => c.CategoriaId == id);

        _logger.LogInformation($"====== GET categorias/produtos/id{id} ======");

        if (categoria == null)
        {
            _logger.LogInformation($"====== GET categorias/produtos/id{id} Not Found ======");
            return NotFound($"Categoria/id{id} não encontrada"); // 404 Not Found se não houver a categoria
        }

        return Ok(categoria); // 200 OK com a categoria encontrada
    }

    // POST
    [HttpPost] // define o método HTTP POST para este endpoint
    public ActionResult Post(Categoria c)
    {
        if (c == null)
        {
            _logger.LogWarning("Dados inválidos!");
            return BadRequest("Dados inválidos!"); // 400 Bad Request se não houver a categoria
        }

        var categoriaCriada = _uof.CategoriaRepository.Create(c);
        _uof.Commit();

        return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaCriada.CategoriaId }, categoriaCriada);
    }

    // PUT
    [HttpPut("{id:int}")] // define o método HTTP PUT para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    public ActionResult Put(int id, Categoria c)
    {
        if (id != c.CategoriaId)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos!");
        }

        _uof.CategoriaRepository.Update(c);
        _uof.Commit();

        return Ok(c);
    }

    // DELETE
    [HttpDelete("{id:int}")] // define o método HTTP DELETE para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    public ActionResult Delete(int id)
    {
        var categoria = _uof.CategoriaRepository.GetById(c => c.CategoriaId == id);

        if (categoria == null)
        {
            _logger.LogWarning($"Categoria id {id} não encontrada!");
            return NotFound($"Categoria id {id} não encontrada!");
        }

        var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);
        _uof.Commit();

        return Ok(categoriaExcluida);
    }
}