using Microsoft.AspNetCore.Mvc;

namespace API.NET9.Controllers
{
    // oferece e permite uma série de recursos que facilitam a construção e funcionamento das APIs
    // (e vai ajudar a distinguir entre controladores de API e controladores MVC)
    // com ele, vamos ter acesso aos recursos de roteamento, validação automática, bindings automaticas, respostas no formato padrão, etc.
    [ApiController]

    [Route("[controller]")]
    // definindo os prefixos de rota p/ os métodos do controlador
    // ("[controller]" será substituído pelo nome do controlador, neste caso "WeatherForecast")

    public class WeatherForecastController : ControllerBase
    // herda de ControllerBase, que é a classe base para controladores de API (o controlador vai lidar com requisições HTTP)
    {
        private static readonly string[] Summaries = new[]
        {
            // array com as previsões do tempo
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        // injeção de dependência do ILogger<WeatherForecastController>
        private readonly ILogger<WeatherForecastController> _logger;
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        // mapeando o método como Action
        [HttpGet(Name = "GetWeatherForecast")]
        // esse "Name" é opcional, o que vai valer é a rota definida no [Route]
        // p/ deixar de ser opcional, basta remover o "Name" do atributo

        // vai receber uma requisição e gerar previsões de tempo aleatórias
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1 , 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)) ,
                TemperatureC = Random.Shared.Next(-20 , 55) ,
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("Teste")]
        public string GetSaudacoes()
        {
            return $"{DateTime.Now.ToShortTimeString()} - Bem-Vindo a minha API";
        }
    }
}
