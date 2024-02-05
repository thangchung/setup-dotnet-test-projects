using MassTransit;

namespace DNP.PeopleService.Consumers;

public record SomethingDone(string GreatMessage);

public class SomethingDoneConsumer(ILogger<SomethingDoneConsumer> logger) :
    IConsumer<SomethingDone>
{
    readonly ILogger<SomethingDoneConsumer> _logger = logger;

    public Task Consume(ConsumeContext<SomethingDone> context)
    {
        _logger.LogInformation("Message: {Message}", context.Message.GreatMessage);
        return Task.CompletedTask;
    }
}