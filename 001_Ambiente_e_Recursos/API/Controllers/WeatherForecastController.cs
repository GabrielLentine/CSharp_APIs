using Microsoft.AspNetCore.Mvc;

namespace API.NET9.Controllers
{
    // oferece e permite uma s�rie de recursos que facilitam a constru��o e funcionamento das APIs
    // (e vai ajudar a distinguir entre controladores de API e controladores MVC)
    // com ele, vamos ter acesso aos recursos de roteamento, valida��o autom�tica, bindings automaticas, respostas no formato padr�o, etc.
    [ApiController]

    [Route("[controller]")]
    // definindo os prefixos de rota p/ os m�todos do controlador
    // ("[controller]" ser� substitu�do pelo nome do controlador, neste caso "WeatherForecast")

    public class WeatherForecastController : ControllerBase
    // herda de ControllerBase, que � a classe base para controladores de API (o controlador vai lidar com requisi��es HTTP)
    {
        private static readonly string[] Summaries = new[]
        {
            // array com as previs�es do tempo
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        // inje��o de depend�ncia do ILogger<WeatherForecastController>
        private readonly ILogger<WeatherForecastController> _logger;
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        // mapeando o m�todo como Action
        [HttpGet(Name = "GetWeatherForecast")]
        // esse "Name" � opcional, o que vai valer � a rota definida no [Route]
        // p/ deixar de ser opcional, basta remover o "Name" do atributo

        // vai receber uma requisi��o e gerar previs�es de tempo aleat�rias
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
