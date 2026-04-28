using CatagoloAPI.Controllers;
using CatagoloAPI.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogoxUnitTests.UnitTests.Classes;

// a classe está sendo configurada p/ usar uma instância compartilhada de ProdutosUnitTestController, ou seja, os testes dentro dessa classe vão usar a mesma instância de ProdutosUnitTestController,
// o que é útil para evitar a criação de múltiplas instâncias e garantir que os testes sejam executados de forma consistente
public class DeleteProdutosUnitTests : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;
    public DeleteProdutosUnitTests(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(controller.repository , controller.mapper);
    }

    [Fact]
    public async Task DeleteProduto_Return_OkResult()
    {
        // arrange
        var prodId = 15;

        // act
        var result = await _controller.Delete(prodId);

        // assert
        result.Should().NotBeNull(); // Verifica se o resultado não é nulo
        result.Result.Should().BeOfType<OkObjectResult>(); // Verifica se o resultado é do tipo OkObjectResult
    }

    [Fact]
    public async Task DeleteProduto_Return_NotFound()
    {
        // arrange
        var prodId = 999;

        // act
        var result = await _controller.Delete(prodId);

        // assert
        result.Should().NotBeNull(); // Verifica se o resultado não é nulo
        result.Result.Should().BeOfType<NotFoundResult>(); // Verifica se o resultado é do tipo NotFoundResult
    }
}
