// estou usando o Scalar p/ visualizar a API pelo navegador
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// MapGet com a rota "/weatherforecast" e a lógica do endpoint
app.MapGet("/weatherforecast" , () =>
{
    var forecast = Enumerable.Range(1 , 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)) ,
            Random.Shared.Next(-20 , 55) ,
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// meu endpoint adicional
app.MapGet("/bemvindo" , () => "Bem-Vindo a minha Minimal API");

app.Run();

// o record é um tipo de classe otimizada p/ representar dados imutáveis de forma simples
internal record WeatherForecast(DateOnly Date , int TemperatureC , string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
