using AutoMapper;
using CatagoloAPI.DTOs;
using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using CatagoloAPI.Repositories;
using CatagoloAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using X.PagedList;

namespace CatagoloAPI.Controllers;

[Route("[controller]")] // define a rota base para este controlador (usando o nome do controlador -> '/categorias')
[ApiController] // indica que este controlador é um controlador de API
public class CategoriasController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IUnityOfWork _uof;
    // private readonly IRepository<Categoria> _repository;
    // private readonly ICategoriaRepository _repository;

    public CategoriasController(IUnityOfWork uof, IConfiguration configuration,
        ILogger<CategoriasController> logger, IMapper mapper)
    {
        _uof = uof;
        _mapper = mapper;

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
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategorias()
    {
        _logger.LogInformation("====== GET categorias ======");

        var categorias = await _uof.CategoriaRepository.GetAllAsync();

        if (categorias == null)
        {
            _logger.LogInformation("Erro 404: categoria não encontrada");
            return NotFound();
        }

        var categoriasDto = new List<CategoriaDTO>();
        foreach (var c in categorias)
        {
            var categoriaDto = new CategoriaDTO
            {
                CategoriaId = c.CategoriaId,
                CategoriaNome = c.CategoriaNome,
                CategoriaImagemUrl = c.CategoriaImagemUrl
            };

            categoriasDto.Add(categoriaDto);
        }

        return Ok(categoriasDto);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetPagination([FromQuery] CategoriasParameters categoriasParameters)
    {
        var categorias = await _uof.CategoriaRepository.GetCategoriasAsync(categoriasParameters);

        return ObterCategorias(categorias);
    }

    [HttpGet("filter/nome/pagination")]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetFilterNomePagination([FromQuery] CategoriasFiltroNome categoriasFiltroNome)
    {
        var categorias = await _uof.CategoriaRepository.GetCategoriasFiltroNomeAsync(categoriasFiltroNome);
        return ObterCategorias(categorias);
    }

    // GET ID
    [HttpGet("{id:int}",
        Name = "ObterCategoria")] // define o método HTTP GET para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    // o Name define um nome para a rota, que pode ser usado para referenciar esta rota em outros lugares (como no método Post)
    public async Task<ActionResult<CategoriaDTO>> GetId(int id)
    {
        // throw new Exception("Exceção ao retornar o produto pelo ID");

        var categoria = await _uof.CategoriaRepository.GetByIdAsync(c => c.CategoriaId == id);

        _logger.LogInformation($"====== GET categorias/produtos/id{id} ======");

        if (categoria == null)
        {
            _logger.LogInformation($"====== GET categorias/produtos/id{id} Not Found ======");
            return NotFound($"Categoria/id{id} não encontrada"); // 404 Not Found se não houver a categoria
        }

        // mapeamento manual entre o objeto DTO e categoria
        var categoriaDto = new CategoriaDTO
        {
            CategoriaId = categoria.CategoriaId,
            CategoriaNome = categoria.CategoriaNome,
            CategoriaImagemUrl = categoria.CategoriaImagemUrl
        };

        return Ok(categoriaDto); // 200 OK com a categoria encontrada
    }

    // POST
    [HttpPost] // define o método HTTP POST para este endpoint
    public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
    {
        if (categoriaDto == null)
        {
            _logger.LogWarning("Dados inválidos!");
            return BadRequest("Dados inválidos!"); // 400 Bad Request se não houver a categoria
        }

        var categoria = new Categoria
        {
            CategoriaId = categoriaDto.CategoriaId,
            CategoriaNome = categoriaDto.CategoriaNome,
            CategoriaImagemUrl = categoriaDto.CategoriaImagemUrl
        };

        var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
        await _uof.CommitAsync();

        var novaCategoriaDto = new CategoriaDTO
        {
            CategoriaId = categoriaCriada.CategoriaId,
            CategoriaNome = categoriaCriada.CategoriaNome,
            CategoriaImagemUrl = categoriaCriada.CategoriaImagemUrl
        };

        return new CreatedAtRouteResult("ObterCategoria", new { id = novaCategoriaDto.CategoriaId }, novaCategoriaDto);
    }

    // PUT
    [HttpPut("{id:int}")] // define o método HTTP PUT para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
    {
        if (id != categoriaDto.CategoriaId)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos!");
        }

        var categoria = new Categoria
        {
            CategoriaId = categoriaDto.CategoriaId,
            CategoriaNome = categoriaDto.CategoriaNome,
            CategoriaImagemUrl = categoriaDto.CategoriaImagemUrl
        };

        var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);
        await _uof.CommitAsync();

        var categoriaAtualzadaDto = new CategoriaDTO
        {
            CategoriaId = categoriaAtualizada.CategoriaId,
            CategoriaNome = categoriaAtualizada.CategoriaNome,
            CategoriaImagemUrl = categoriaAtualizada.CategoriaImagemUrl
        };

        return Ok(categoriaAtualzadaDto);
    }

    // DELETE
    [HttpDelete("{id:int}")] // define o método HTTP DELETE para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    public async Task<ActionResult<CategoriaDTO>> Delete(int id)
    {
        var categoria = await _uof.CategoriaRepository.GetByIdAsync(c => c.CategoriaId == id);

        if (categoria == null)
        {
            _logger.LogWarning($"Categoria id {id} não encontrada!");
            return NotFound($"Categoria id {id} não encontrada!");
        }

        var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);
        await _uof.CommitAsync();

        var categoriaExcluidaDto = new CategoriaDTO
        {
            CategoriaId = categoriaExcluida.CategoriaId,
            CategoriaNome = categoriaExcluida.CategoriaNome,
            CategoriaImagemUrl = categoriaExcluida.CategoriaImagemUrl
        };

        return Ok(categoriaExcluidaDto);
    }

    private ActionResult<IEnumerable<Categoria>> ObterCategorias(IPagedList<Categoria> categorias)
    {
        var metadata = new
        {
            categorias.Count ,
            categorias.PageSize ,
            categorias.PageCount ,
            categorias.TotalItemCount ,
            categorias.HasNextPage ,
            categorias.HasPreviousPage
        };
        Response.Headers.Append("X-Pagination" , JsonConvert.SerializeObject(metadata));

        var categoriaDTO = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);
        return Ok(categoriaDTO);
    }
}