using System.Text.Json.Serialization;
using CatagoloAPI.Context;
using CatagoloAPI.Extensions;
using CatagoloAPI.Filters;
using CatagoloAPI.Logs;
using CatagoloAPI.Repositories;
using CatagoloAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(op => { op.Filters.Add(typeof(ApiExceptionFilter)); })
    .AddJsonOptions(op => { op.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configurando o banco de dados com MySQL
var mySqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// obtendo a string de conex�o do arquivo appsettings.json

// definindo o contexto + o provedor (MySQL) + a string de conex�o = DI do AppDbContext aonde eu precisar dela (neste caso, nos reposit�rios)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnectionString,
        ServerVersion.AutoDetect(mySqlConnectionString)));

// registrando o filtro global ApiLoggingFilter para registrar logs de requisi��es e respostas
builder.Services.AddScoped<ApiLoggingFilter>();

// registrando o servi�o MeuServico como uma dependência transitória (uma nova inst�ncia ser� criada a cada vez que for injetada)
builder.Services.AddTransient<IMeuServico, MeuServico>();

// registrando o repositório para as categorias
// toda vez que eu referencias ICategoriaRepository, eu vou receber uma instância com as implementações presentes em CategoriaRepository
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

// registrando o repositório para os produtos
// toda vez que eu referencias IProdutoRepository, eu vou receber uma instância com as implementações presentes em ProdutoRepository
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();

// registrando o repositório genérico
// toda vez que eu referencias IRepository, eu vou receber uma instância com as implementações presentes em Repository, de acordo com a classe a ser chamada
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// registrando o repositório de UnityOfWork
builder.Services.AddScoped<IUnityOfWork, UnityOfWork>();

var valor1 = builder.Configuration["chave1"];
var valor2 = builder.Configuration["secao1:chave2"];
// obtendo os valores das chaves do arquivo appsettings.json

// registrando a configura��o do provedor de log personalizado
builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // middleware para gerar a documenta��o Swagger
    app.UseSwaggerUI(); // middleware para exibir a interface do Swagger
    // app.UseDeveloperExceptionPage(); -> middleware para exibir a p�gina de erro detalhada
    app.ConfigureExceptionHandler(); // middleware para tratar exce��es de forma personalizada
}

app.UseHttpsRedirection(); // middleware para redirecionar requisi��es HTTP para HTTPS

// app.UseAuthentication(); -> middleware para autenticar requisi��es (necess�rio se voc� tiver autentica��o configurada)
app.UseAuthorization(); // middleware para autorizar requisi��es (necess�rio se voc� tiver autentica��o/autoriza��es configuradas)

app.MapControllers(); // mapear os controladores para as rotas
app.Run();