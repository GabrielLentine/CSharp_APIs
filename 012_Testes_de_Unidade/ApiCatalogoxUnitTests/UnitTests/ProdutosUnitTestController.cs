using AutoMapper;
using CatagoloAPI.Context;
using CatagoloAPI.DTOs.Mapping;
using CatagoloAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogoxUnitTests.UnitTests;
public class ProdutosUnitTestController
{
    public IUnityOfWork repository;
    public IMapper mapper;
    public static DbContextOptions<AppDbContext> dbContextOptions { get; }
    public static string connectionString = "Server=localhost;DataBase=CatalogoDB;Uid=root;Pwd=P57u9j";

    // executado uma vez, posteriormente vai ficar salvo na memória, ou seja, não precisa ser executado toda vez que for instanciada a classe
    static ProdutosUnitTestController()
    {
        // Configura as opções do DbContext para usar o MySQL
        dbContextOptions = new DbContextOptionsBuilder<AppDbContext>().UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)).Options;
    }

    public ProdutosUnitTestController()
    {
        // Configura o AutoMapper
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });
        mapper = config.CreateMapper(); // cria o mapeador com base na configuração definida

        var context = new AppDbContext(dbContextOptions); // cria uma instância do contexto do banco de dados usando as opções configuradas
        repository = new UnityOfWork(context); // cria uma instância do repositório usando o contexto do banco de dados
    }
}
