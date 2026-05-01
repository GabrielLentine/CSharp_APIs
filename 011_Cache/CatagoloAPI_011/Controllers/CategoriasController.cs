using AutoMapper;
using CatagoloAPI.DTOs;
using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using CatagoloAPI.Repositories;
using CatagoloAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using X.PagedList;

namespace CatagoloAPI.Controllers;

[Route("[controller]")] // define a rota base para este controlador (usando o nome do controlador -> '/categorias')
[ApiController] // indica que este controlador é um controlador de API
[EnableRateLimiting("FixedWindow")]
public class CategoriasController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IUnityOfWork _uof;
    private readonly IMemoryCache _cache;
    private const string CacheCategoriasKey = "CacheCategorias";
    // private readonly IRepository<Categoria> _repository;
    private readonly ICategoriaRepository _categoriaRepository;

    public CategoriasController(IUnityOfWork uof , IConfiguration configuration , ILogger<CategoriasController> logger , IMapper mapper , IMemoryCache cache , ICategoriaRepository categoriaRepository)
    {
        _uof = uof;
        _mapper = mapper;
        _cache = cache;

        // injeta a configuração do aplicativo no controlador
        _configuration = configuration;

        // injeta o logger no controlador
        _logger = logger;

        _categoriaRepository = categoriaRepository;
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
    [DisableRateLimiting]
    // [Authorize] só será acessado por usuários autorizados
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategorias()
    {
        _logger.LogInformation("====== GET categorias ======");

        // tenta obter as categorias do cache (se tiver, ele retorna o que está presente no cache, caso contrário, ele executa o código dentro do if para buscar as categorias do banco de dados)
        if(!_cache.TryGetValue(CacheCategoriasKey , out IEnumerable<Categoria>? categorias))
        {
            categorias = await _uof.CategoriaRepository.GetAllAsync();

            if(categorias is null || !categorias.Any())
            {
                _logger.LogInformation("Erro 404: categoria não encontrada");
                return NotFound();
            }
            SetCache(CacheCategoriasKey , categorias); // armazena as categorias no cache com as opções definidas
        }
        return Ok(categorias);
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
    [HttpGet("{id:int}" , Name = "ObterCategoria")] // define o método HTTP GET para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    // o Name define um nome para a rota, que pode ser usado para referenciar esta rota em outros lugares (como no método Post)
    public async Task<ActionResult<CategoriaDTO>> GetId(int id)
    {
        var cacheKey = GetCategoriaCacheKey(id); // gera a chave de cache específica para a categoria com base no ID fornecido

        // tenta obter as categorias do cache (se tiver, ele retorna o que está presente no cache, caso contrário, ele executa o código dentro do if para buscar as categorias do banco de dados)
        if(!_cache.TryGetValue(cacheKey , out Categoria? categoria))
        {
            categoria = await _categoriaRepository.GetByIdAsync(c => c.CategoriaId == id);

            if(categoria is null)
            {
                _logger.LogInformation("Erro 404: categoria não encontrada");
                return NotFound();
            }
            SetCache(cacheKey , categoria); // armazena a categoria no cache com as opções definidas
        }
        return Ok(categoria);
    }

    // POST
    [HttpPost] // define o método HTTP POST para este endpoint
    public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
    {
        if(categoriaDto == null)
        {
            _logger.LogWarning("Dados inválidos!");
            return BadRequest("Dados inválidos!"); // 400 Bad Request se não houver a categoria
        }

        var categoria = new Categoria
        {
            CategoriaId = categoriaDto.CategoriaId ,
            CategoriaNome = categoriaDto.CategoriaNome ,
            CategoriaImagemUrl = categoriaDto.CategoriaImagemUrl
        };

        var categoriaCriada = _uof.CategoriaRepository.Create(categoria);

        InvalidateCacheAfterChange(categoriaCriada.CategoriaId , categoriaCriada); // invalida o cache após a criação da categoria para garantir que a nova categoria seja refletida na próxima consulta

        await _uof.CommitAsync();

        var novaCategoriaDto = new CategoriaDTO
        {
            CategoriaId = categoriaCriada.CategoriaId ,
            CategoriaNome = categoriaCriada.CategoriaNome ,
            CategoriaImagemUrl = categoriaCriada.CategoriaImagemUrl
        };

        return new CreatedAtRouteResult("ObterCategoria" , new { id = novaCategoriaDto.CategoriaId } , novaCategoriaDto);
    }

    // PUT
    [HttpPut("{id:int}")] // define o método HTTP PUT para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    public async Task<ActionResult<CategoriaDTO>> Put(int id , CategoriaDTO categoriaDto)
    {
        if(id != categoriaDto.CategoriaId)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos!");
        }

        var categoria = new Categoria
        {
            CategoriaId = categoriaDto.CategoriaId ,
            CategoriaNome = categoriaDto.CategoriaNome ,
            CategoriaImagemUrl = categoriaDto.CategoriaImagemUrl
        };

        var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);

        InvalidateCacheAfterChange(categoriaAtualizada.CategoriaId , categoriaAtualizada); // invalida o cache após a atualização da categoria para garantir que as alterações sejam refletidas na próxima consulta

        await _uof.CommitAsync();

        var categoriaAtualzadaDto = new CategoriaDTO
        {
            CategoriaId = categoriaAtualizada.CategoriaId ,
            CategoriaNome = categoriaAtualizada.CategoriaNome ,
            CategoriaImagemUrl = categoriaAtualizada.CategoriaImagemUrl
        };

        return Ok(categoriaAtualzadaDto);
    }

    // DELETE
    [HttpDelete("{id:int}")] // define o método HTTP DELETE para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    [Authorize(Policy = "AdminOnly")] // apenas usuários com a role Admin podem acessar este endpoint
    public async Task<ActionResult<CategoriaDTO>> Delete(int id)
    {
        var categoria = await _uof.CategoriaRepository.GetByIdAsync(c => c.CategoriaId == id);

        if(categoria == null)
        {
            _logger.LogWarning($"Categoria id {id} não encontrada!");
            return NotFound($"Categoria id {id} não encontrada!");
        }

        var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);

        InvalidateCacheAfterChange(categoriaExcluida.CategoriaId); // invalida o cache após a exclusão da categoria para garantir que as alterações sejam refletidas na próxima consulta

        await _uof.CommitAsync();

        var categoriaExcluidaDto = new CategoriaDTO
        {
            CategoriaId = categoriaExcluida.CategoriaId ,
            CategoriaNome = categoriaExcluida.CategoriaNome ,
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

    private string GetCategoriaCacheKey(int id) => $"CacheCategoria_{id}"; // método auxiliar para gerar a chave de cache específica para uma categoria com base no ID fornecido

    private void SetCache<T>(string key , T data)
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30) , // o cache expira após 30 segundos
            SlidingExpiration = TimeSpan.FromSeconds(15) , // o cache expira se não for acessado por 15 segundos
            Priority = CacheItemPriority.High // prioridade alta para o cache
        };
        _cache.Set(key , data , cacheOptions); // armazena os dados no cache com as opções definidas
    }

    private void InvalidateCacheAfterChange(int id , Categoria? categoria = null)
    {
        _cache.Remove(CacheCategoriasKey); // remove o cache de todas as categorias para garantir que as alterações sejam refletidas na próxima consulta
        _cache.Remove(GetCategoriaCacheKey(id)); // remove o cache específico da categoria com base no ID fornecido para garantir que as alterações sejam refletidas na próxima consulta

        if(categoria != null) SetCache(GetCategoriaCacheKey(id) , categoria); // se a categoria for fornecida, armazena a categoria atualizada no cache com as opções definidas
    }
}