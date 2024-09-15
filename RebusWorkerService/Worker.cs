using Rebus.Auditing.Messages;
using Rebus.Config;
using Rebus.Retry.Simple;
using Rebus.Transport.FileSystem;

namespace RebusWorkerService;

/// <summary> This approach is based on Mogens' answer here: https://stackoverflow.com/a/60631521/2608 </summary>
public class Worker : BackgroundService
{
    private readonly IServiceCollection _services = new ServiceCollection();

    public Worker()
    {
        _services.AddLogging(logging => logging.AddConsole());
        
        // Using Rebus with the file system transport, hard coding the configuration values for simplicity.
        // We send error messages to an error queue and use an audit queue so you can see the messages that have been processed.
        _services.AddRebus(
            rebus => rebus
               .Logging  (l => l.Console())
               .Transport(t => t.UseFileSystem("c:/rebus", "MainQueue"))
               .Options  (t => t.RetryStrategy(errorQueueName: "ErrorQueue"))
               .Options  (t => t.EnableMessageAuditing(auditQueue: "AuditQueue"))
        );        
        
        _services.AutoRegisterHandlersFromAssemblyOf<Worker>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var provider = _services.BuildServiceProvider();
        provider.StartRebus();

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}