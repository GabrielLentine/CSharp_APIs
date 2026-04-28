using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatagoloAPI.Migrations
{
    /// <inheritdoc />
    public partial class PopulaCategorias : Migration
    {
        /// <inheritdoc />
        // colocando algumas categorias p/ popular o banco de dados de categorias
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("Insert into Categorias(CategoriaNome, CategoriaImagemUrl) Values('Lanches', 'lanches.jpg')");
            mb.Sql("Insert into Categorias(CategoriaNome, CategoriaImagemUrl) Values('Bebidas', 'bebidas.jpg')");
            mb.Sql("Insert into Categorias(CategoriaNome, CategoriaImagemUrl) Values('Sobremesas', 'sobremesas.jpg')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Categorias");
        }
    }
}
