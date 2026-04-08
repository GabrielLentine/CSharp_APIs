using CatagoloAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CatagoloAPI.Context;

// representa uma sessão com o banco de dados sendo a ponte entre as entidades de domínio e o banco de dados
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    // esse parâmetro (AppDbContextOptions) é passado para a classe base DbContext, que é responsável por configurar o contexto do banco de dados
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSet representa uma coleção de entidades do tipo especificado que podem ser consultadas ou salvas no banco de dados
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // chama o método da classe base para garantir que a configuração padrão do Identity seja aplicada
    }
}
