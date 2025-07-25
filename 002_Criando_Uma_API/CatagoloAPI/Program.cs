using CatagoloAPI.Context;
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

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
