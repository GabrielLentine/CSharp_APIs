using CatagoloAPI.Controllers;
using CatagoloAPI.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogoxUnitTests.UnitTests.Classes;

// a classe está sendo configurada p/ usar uma instância compartilhada de ProdutosUnitTestController, ou seja, os testes dentro dessa classe vão usar a mesma instância de ProdutosUnitTestController,
// o que é útil para evitar a criação de múltiplas instâncias e garantir que os testes sejam executados de forma consistente
public class PostProdutosUnitTests : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;
    public PostProdutosUnitTests(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(controller.repository , controller.mapper);
    }

    [Fact]
    public async Task PostProduto_Return_CreatedStatusCode()
    {
        // arrange
        var novoProdutoDto = new ProdutoDTO
        {
            ProdutoNome = "Produto Teste" ,
            ProdutoDescricao = "Descrição do Produto Teste" ,
            ProdutoPreco = 99.99m ,
            ProdutoImagemUrl = "http://exemplo.com/produto-teste.jpg" ,
            CategoriaId = 1
        };

        // act
        var data = await _controller.Post(novoProdutoDto);

        // assert
        var createdResult = data.Result.Should().BeOfType<CreatedAtRouteResult>();
        createdResult.Subject.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task PostProduto_Return_BadRequest()
    {
        // arrange
        ProdutoDTO prod = null;

        // act
        var data = await _controller.Post(prod);

        // assert
        var badRequestResult = data.Result.Should().BeOfType<BadRequestResult>();
        badRequestResult.Subject.StatusCode.Should().Be(400);
    }
}
