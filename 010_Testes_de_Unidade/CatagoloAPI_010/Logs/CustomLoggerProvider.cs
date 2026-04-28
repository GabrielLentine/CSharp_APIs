using System.Collections.Concurrent;

namespace CatagoloAPI.Logs;
public class CustomLoggerProvider : ILoggerProvider
{
    // Configuração do provedor de log personalizada
    readonly CustomLoggerProviderConfiguration loggerConfig;

    // dicionário de loggers
    readonly ConcurrentDictionary<string , CustomerLogger> loggers = new ConcurrentDictionary<string , CustomerLogger>();

    public CustomLoggerProvider(CustomLoggerProviderConfiguration config)
    {
        loggerConfig = config;
    }

    // criar um log para uma categoria específica
    public ILogger CreateLogger(string categoryName)
    {
        // vai retornar um logger existente ou criar um novo se não existir
        return loggers.GetOrAdd(categoryName , name => new CustomerLogger(name , loggerConfig));
    }

    public void Dispose()
    {
        // liberar recursos quando o provedor for descartado
        loggers.Clear();
    }
}
