using CatagoloAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatagoloAPI.Context;

// representa uma sessão com o banco de dados sendo a ponte entre as entidades de domínio e o banco de dados
public class AppDbContext : DbContext
{
    // esse parâmetro (AppDbContextOptions) é passado para a classe base DbContext, que é responsável por configurar o contexto do banco de dados
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSet representa uma coleção de entidades do tipo especificado que podem ser consultadas ou salvas no banco de dados
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
}
