using CatagoloAPI.Context;
using CatagoloAPI.Extensions;
using CatagoloAPI.Filters;
using CatagoloAPI.Logs;
using CatagoloAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions
                                 (options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configurando o banco de dados com MySQL
var mySqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// obtendo a string de conexão do arquivo appsettings.json

// definindo o contexto + o provedor (MySQL) + a string de conexão = DI do AppDbContext aonde eu precisar dela (neste caso, nos repositórios)
builder.Services.AddDbContext<AppDbContext>(options =>
                                            options.UseMySql(mySqlConnectionString ,
                                            ServerVersion.AutoDetect(mySqlConnectionString)));

// registrando o filtro global ApiLoggingFilter para registrar logs de requisições e respostas
builder.Services.AddScoped<ApiLoggingFilter>();

builder.Services.AddTransient<IMeuServico , MeuServico>();
// registrando o serviço MeuServico como uma dependência transitória (uma nova instância será criada a cada vez que for injetada)

var valor1 = builder.Configuration["chave1"];
var valor2 = builder.Configuration["secao1:chave2"];
// obtendo os valores das chaves do arquivo appsettings.json

// registrando a configuração do provedor de log personalizado
builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger(); // middleware para gerar a documentação Swagger
    app.UseSwaggerUI(); // middleware para exibir a interface do Swagger
    // app.UseDeveloperExceptionPage(); -> middleware para exibir a página de erro detalhada
    app.ConfigureExceptionHandler(); // middleware para tratar exceções de forma personalizada
}

app.UseHttpsRedirection(); // middleware para redirecionar requisições HTTP para HTTPS

// app.UseAuthentication(); -> middleware para autenticar requisições (necessário se você tiver autenticação configurada)
app.UseAuthorization(); // middleware para autorizar requisições (necessário se você tiver autenticação/autorizações configuradas)

app.MapControllers(); // mapear os controladores para as rotas
app.Run();
