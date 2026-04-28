using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatagoloAPI.Migrations
{
    /// <inheritdoc />
    public partial class PopulaProdutos : Migration
    {
        /// <inheritdoc />
        // colocando alguns produtos p/ popular o banco de dados de produtos
        protected override void Up(MigrationBuilder mb)
        {
            // lanches
            mb.Sql("Insert into Produtos(ProdutoNome, ProdutoDescricao, ProdutoPreco, ProdutoImagemUrl, ProdutoEstoque, DataCadastro, CategoriaId)" +
            "values('Lanche de Atum', 'Lanche de Atum com Maionese', 8.50, 'lanche-atum.jpg', 15, now(), 1)");

            // bebidas
            mb.Sql("Insert into Produtos(ProdutoNome, ProdutoDescricao, ProdutoPreco, ProdutoImagemUrl, ProdutoEstoque, DataCadastro, CategoriaId)" +
            "values('Coca-Cola Diet', 'Refrigerante de Cola 2L', 17.45, 'coca-cola.jpg', 50, now(), 2)");

            // sobremesas
            mb.Sql("Insert into Produtos(ProdutoNome, ProdutoDescricao, ProdutoPreco, ProdutoImagemUrl, ProdutoEstoque, DataCadastro, CategoriaId)" +
            "values('Sorvete de Morango', 'Sorvete de Morango 2L da KIBOM', 32.99, 'sorvete-morango.jpg', 12, now(), 3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Produtos");
        }
    }
}
