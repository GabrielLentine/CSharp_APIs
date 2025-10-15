namespace CatagoloAPI.Logs;
public class CustomLoggerProviderConfiguration
{
    // define o nível mínimo de log a ser registrado
    public LogLevel LogLevel { get; set; } = LogLevel.Warning;

    // define o ID do evento de log
    public int EventId { get; set; } = 0;
}
