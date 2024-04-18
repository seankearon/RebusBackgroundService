using Messages;
using Rebus.Handlers;

namespace RebusService;

public class SimpleMessageHandler : IHandleMessages<SimpleMessage>
{
    private readonly ILogger<SimpleMessageHandler> _logger;

    public SimpleMessageHandler(ILogger<SimpleMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SimpleMessage message)
    {
        _logger.LogInformation("Received message: {message}", message.Text);
        return Task.CompletedTask;
    }
}