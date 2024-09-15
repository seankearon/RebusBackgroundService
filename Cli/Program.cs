using Messages;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.Transport.FileSystem;

Directory.CreateDirectory("c:/rebus/MainQueue"); // Rebus doesn't create the folder in one-way mode.  

var services = new ServiceCollection();

services.AddRebus(
    rebus => rebus
       .Logging  (l => l.Console())
       .Routing(r => r.TypeBased().Map<SimpleText>("MainQueue"))
       .Transport(t => t.UseFileSystemAsOneWayClient("c:/rebus"))
       .Options  (t => t.RetryStrategy(errorQueueName: "ErrorQueue"))
);        

var provider = services.BuildServiceProvider();
provider.StartRebus();
var bus = provider.GetRequiredService<IBus>();

Console.WriteLine("Enter the message text (press Enter with no content to quit):");
        
while (true)
{
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
    {
        break;
    }
       
    bus.Send(new SimpleText{Message = input});
}

Console.WriteLine("Goodbye!");
