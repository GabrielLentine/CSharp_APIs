using CatagoloAPI.Context;
using CatagoloAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatagoloAPI.Controllers;

[Route("[controller]")] // define a rota base para este controlador (usando o nome do controlador -> '/categorias')
[ApiController] // indica que este controlador é um controlador de API
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;
    public CategoriasController(AppDbContext context)
    {
        // injeta o contexto do banco de dados no controlador
        _context = context;
    }

    // GET
    [HttpGet] // define o método HTTP GET para este endpoint
    public ActionResult<IEnumerable<Categoria>> Get()
    {
        try
        {
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
        // return _context.Categorias.Include(p => p.Produtos).AsNoTracking().ToList();
        return _context.Categorias.Include(p => p.Produtos).Where(c => c.CategoriaId <= 5).AsNoTracking().ToList();
        // retorna as categorias com os produtos relacionados, limitando até o ID 5
    }

    // GET ID
    [HttpGet("{id:int}" , Name = "ObterCategoria")] // define o método HTTP GET para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    // o Name define um nome para a rota, que pode ser usado para referenciar esta rota em outros lugares (como no método Post)
    public ActionResult<Categoria> GetId(int id)
    {
        var categoria = _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);
        if(categoria == null) return NotFound(); // 404 Not Found se não houver a categoria
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
