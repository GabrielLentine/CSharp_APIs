using CatagoloAPI.Controllers;
using CatagoloAPI.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogoxUnitTests.UnitTests.Classes;

// a classe está sendo configurada p/ usar uma instância compartilhada de ProdutosUnitTestController, ou seja, os testes dentro dessa classe vão usar a mesma instância de ProdutosUnitTestController,
// o que é útil para evitar a criação de múltiplas instâncias e garantir que os testes sejam executados de forma consistente
public class PutProdutosUnitTests : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;
    public PutProdutosUnitTests(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(controller.repository , controller.mapper);
    }

    [Fact]
    public async Task PutProduto_Update_Return_OkResult()
    {
        // arrange
        var prodId = 15;

        var atualiazarProduto = new ProdutoDTO
        {
            ProdutoId = prodId ,
            ProdutoNome = "Produto Atualizado" ,
            ProdutoDescricao = "Descrição do Produto Atualizado" ,
            ProdutoImagemUrl = "https://example.com/produto-atualizado.jpg" ,
            CategoriaId = 1
        };

        // act
        var result = await _controller.Put(prodId , atualiazarProduto);

        // assert
        result.Should().NotBeNull(); // Verifica se o resultado não é nulo
        result.Result.Should().BeOfType<OkObjectResult>(); // Verifica se o resultado é do tipo OkObjectResult
    }

    [Fact]
    public async Task PutProduto_Update_Return_BadRequest()
    {
        // arrange
        var prodId = 14;

        var atualiazarProduto = new ProdutoDTO
        {
            ProdutoId = 15 ,
            ProdutoNome = "Produto Atualizado" ,
            ProdutoDescricao = "Descrição do Produto Atualizado" ,
            ProdutoImagemUrl = "https://example.com/produto-atualizado.jpg" ,
            CategoriaId = 1
        };

        // act
        var result = await _controller.Put(prodId , atualiazarProduto);

        // assert
        result.Should().NotBeNull(); // Verifica se o resultado não é nulo
        result.Result.Should().BeOfType<BadRequestResult>(); // Verifica se o resultado é do tipo OkObjectResult
    }
}
