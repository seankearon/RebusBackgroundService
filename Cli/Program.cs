
using Messages;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Auditing.Messages;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.Transport.FileSystem;

RebusConfigurer ConfigureRebus(RebusConfigurer rebus) =>
    rebus.Logging(x => x.Console())
       .Transport(x => x.UseFileSystemAsOneWayClient("c:/rebus"))
       .Routing(x => x.TypeBased().Map<SimpleText>("MainQueue"))
       .Options(x => x.RetryStrategy(errorQueueName: "ErrorQueue"))
       .Options(x => x.EnableMessageAuditing(auditQueue: "AuditQueue"));

Directory.CreateDirectory("c:/rebus/MainQueue"); // Rebus doesn't create the folder in one-way mode.  

var services = new ServiceCollection();
services.AddRebus((Func<RebusConfigurer,RebusConfigurer>)ConfigureRebus);

var provider = services.BuildServiceProvider();
IBus bus = provider.GetRequiredService<IBus>();

Console.WriteLine("This is a simple CLI that sends Rebus messages.");
Console.WriteLine("Type your message and click <Enter> to send.");
Console.WriteLine("Type a number and press <Enter> to auto-generate and send that number of test messages.");
Console.WriteLine("Press <Enter> without entering anything to quit.");

var input = Console.ReadLine();
while (!string.IsNullOrEmpty(input))
{
    if (int.TryParse(input, out var number))
    {
        Console.WriteLine($"Sending {number} messages...");
        for (var i = 0; i < number; i++)
        {
            var message = new SimpleText { Message = $"Test message {i + 1}" };
            await bus.Send(message);
        }
        Console.WriteLine($"Sent {number} messages.");
        input = Console.ReadLine();
    }
    else
    {
        var message = new SimpleText { Message = input };
        await bus.Send(message);
        Console.WriteLine($"Message sent: {message.Message}");
        input = Console.ReadLine();
    }
}
