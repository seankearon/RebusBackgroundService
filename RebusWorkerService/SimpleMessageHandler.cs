using Messages;
using Rebus.Handlers;

namespace RebusWorkerService;

public class SimpleTextHandler: IHandleMessages<SimpleText>
{
    public async Task Handle(SimpleText m)
    {
        await Task.Delay(10000); // Pretend we're doing some work!
        Console.WriteLine(m.Message);
    }
}