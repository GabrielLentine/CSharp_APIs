
namespace CatagoloAPI.Logs;
public class CustomerLogger : ILogger
{
    readonly string loggerName;
    readonly CustomLoggerProviderConfiguration loggerConfig;

    public CustomerLogger(string name , CustomLoggerProviderConfiguration config)
    {
        loggerName = name;
        loggerConfig = config;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        // verifica se o nível de log está habilitado conforme a configuração; se não estiver, nada será registrado
        return logLevel == loggerConfig.LogLevel;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        // não implementa escopo de log
        return null;
    }

    public void Log<TState>(LogLevel logLevel , EventId eventId , TState state , Exception? exception , Func<TState , Exception? , string> formatter)
    {
        // registrar uma mensagem de log formatada
        string message = $"{logLevel.ToString()}: {eventId.Id} - {formatter(state , exception)}";

        // EscreverTextoNoArquivo(message);
    }

    //private void EscreverTextoNoArquivo(string message)
    //{
    //    string caminhoArquivo = @"D:\C#\APIs\003_Fundamentos_WebAPI\CatagoloAPI_003\Logs\Lentine.txt";

    //    using(StreamWriter writer = new StreamWriter(caminhoArquivo , true))
    //    {
    //        try
    //        {
    //            writer.WriteLine(message);
    //            writer.Close();
    //        }
    //        catch(Exception e)
    //        {
    //            throw new Exception(e.Message);
    //        }
    //    }
    //}
}
