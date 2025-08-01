﻿using CatagoloAPI.Context;
using CatagoloAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatagoloAPI.Controllers;

[Route("[controller]")] // define a rota base para este controlador (usando o nome do controlador -> '/produtos')
[ApiController] // indica que este controlador é um controlador de API
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;
    public ProdutosController(AppDbContext context)
    {
        // injeta o contexto do banco de dados no controlador
        _context = context;
    }

    // GET
    [HttpGet] // define o método HTTP GET para este endpoint
    // usando IEnumerable eu trabalharei por demanda, ou seja, não trago todos os produtos de uma vez, mas sim conforme a necessidade, sendo assim mais otimizado
    public ActionResult<IEnumerable<Produto>> Get()
    {
        // busca todos os produtos do banco de dados sem rastreamento (AsNoTracking) para melhorar a performance
        // só posso usar o AsNoTracking quando não for necessário rastrear as entidades retornadas (como em consultas de leitura)
        var produtos = _context.Produtos.AsNoTracking().ToList();
        if(produtos == null) return NotFound(); // 404 Not Found se não houver
        return produtos;
    }

    // GET ID
    [HttpGet("{id:int}" , Name = "ObterProduto")] // define o método HTTP GET para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    // o Name define um nome para a rota, que pode ser usado para referenciar esta rota em outros lugares (como no método Post)
    public ActionResult<Produto> GetId(int id)
    {
        // busca um produto específico pelo ID
        var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
        if(produto == null) return NotFound();
        return produto;
    }

    // POST
    [HttpPost] // define o método HTTP POST para este endpoint
    // ActionResult é usado para retornar diferentes tipos de respostas HTTP
    public ActionResult Post(Produto p)
    {
        if(p == null) return BadRequest(); // 400 Bad Request se não houver o produto
        _context.Produtos.Add(p); // adiciona o produto ao contexto do banco de dados
        _context.SaveChanges(); // salva as alterações no banco de dados
        return new CreatedAtRouteResult("ObterProduto" , new { id = p.ProdutoId } , p);
        // retorna uma resposta 201 Created com a rota para o produto recém-criado
    }

    // PUT
    [HttpPut("{id:int}")] // define o método HTTP PUT para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    public ActionResult Put(int id , Produto p)
    {
        if(id != p.ProdutoId) return BadRequest(); // 400 Bad Request se o ID do produto não corresponder ao ID na rota
        _context.Entry(p).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        // marca o produto como modificado no contexto do banco de dados

        _context.SaveChanges();
        return Ok(p);
    }

    // DELETE
    [HttpDelete("{id:int}")] // define o método HTTP DELETE para este endpoint com um parâmetro de rota 'id' do tipo inteiro
    public ActionResult Delete(int id)
    {
        var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
        if(produto == null) return NotFound("Produto não localizado!"); // 404 Not Found se o produto não for encontrado (com uma mensagem)
        _context.Produtos.Remove(produto);
        _context.SaveChanges();
        return Ok(produto);
    }
}
