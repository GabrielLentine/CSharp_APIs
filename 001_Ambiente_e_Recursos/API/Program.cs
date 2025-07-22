/* Ajustando o código p/ usar o SwaggerUI:
 * 1 - Instalar o pacote NuGet "Swashbuckle.AspNetCore" (SwaggerUI);
 * 2 - Adicionar o 'app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json" , "weather api"));';
 * 3 - Em 'launchSettings.json', "launchBrowser": true / "lauchUrl": "swagger".
*/

/* Ajustando o código p/ usar o Scalar:
 * 1 - Incluir o pacote NuGet "Scalar.AspNetCore";
 * 2 - Adicionar o 'using Scalar.AspNetCore;' e 'app.MapScalarApiReference();';
 * 3 - Em 'launchSettings.json', "launchBrowser": true / "lauchUrl": "scalar/v1".
 */

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. -> ConfigureServices()
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline. -> Configure()
if(app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json" , "weather api"));
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
