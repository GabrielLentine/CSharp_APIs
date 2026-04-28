using CatagoloAPI.Controllers;
using CatagoloAPI.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogoxUnitTests.UnitTests.Classes;

// a classe está sendo configurada p/ usar uma instância compartilhada de ProdutosUnitTestController, ou seja, os testes dentro dessa classe vão usar a mesma instância de ProdutosUnitTestController,
// o que é útil para evitar a criação de múltiplas instâncias e garantir que os testes sejam executados de forma consistente
public class GetProdutosUnitTests : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;
    public GetProdutosUnitTests(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(controller.repository, controller.mapper);
    }

    [Fact]
    public async Task GetProdutoById_Return_OkResult() 
    {
        // arrange
        var produtoId = 1; // ID do produto que você deseja testar

        // act
        var data = await _controller.GetId(produtoId);

        // assert (xunit)
        // var okResult = Assert.IsType<OkObjectResult>(data.Result); -> verifica se o resultado esperado é OkObjectResult (200)
        // Assert.Equal(200, okResult.StatusCode); -> verifica se o código de status é 200

        // assert (fluentassertions)
        data.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(200); // verifica se o resultado esperado é OkObjectResult (200) e se o código de status é 200);
    }

    [Fact]
    public async Task GetProdutoById_Return_NotFound() 
    {
        // arrange
        var produtoId = 999; // ID do produto que não existe

        // act
        var data = await _controller.GetId(produtoId);

        // assert
        data.Result.Should().BeOfType<NotFoundResult>().Which.StatusCode.Should().Be(404); // verifica se o resultado esperado é NotFoundResult (404) e se o código de status é 404);
    }

    [Fact]
    public async Task GetProdutoById_Return_BadRequest() 
    {
        // arrange
        int produtoId = -1; // ID do produto inválido

        // act
        var data = await _controller.GetId(produtoId);

        // assert
        data.Result.Should().BeOfType<BadRequestResult>().Which.StatusCode.Should().Be(400); // verifica se o resultado esperado é BadRequestResult (400, sem mensagem) e se o código de status é 400);
    }

    [Fact]
    public async Task GetProdutos_Return_ListOfProdutoDTO() 
    {
        // arrange -> não tem arrange específico para esse teste, pois estamos testando a obtenção de todos os produtos, então não precisamos configurar um cenário específico

        // act
        var data = await _controller.Get();

        // assert
        data.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeAssignableTo<IEnumerable<ProdutoDTO>>().And.NotBeNull();
        // verifica se o resultado esperado é OkObjectResult (200) e se o valor retornado é do tipo IEnumerable<ProdutoDTO> e não é nulo
    }

    [Fact]
    public async Task GetProdutos_Return_BadRequestResult() 
    {
        // act
        var data = await _controller.Get();

        // assert
        data.Result.Should().BeOfType<BadRequestResult>(); // verifica se o resultado esperado é BadRequestResult (400, sem mensagem)
    }
}
