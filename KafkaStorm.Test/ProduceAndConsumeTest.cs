using KafkaStorm.Test.TestEvents;

namespace KafkaStorm.Test;

public class ProduceAndConsumeTest : TestBase
{
    [Fact]
    public async Task Produce_Should_Raise_No_Exception()
    {
        await Produce(new HelloEvent("test body"));
    }
}