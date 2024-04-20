using KafkaStorm.Registration;
using KafkaStorm.Test.TestConsumers;

namespace KafkaStorm.Test;

public class AddingConsumerFromAssemblyTest : TestBase
{
    [Fact]
    public void ConsumerShouldHaveBeenAdded()
    {
        var result = ConsumerRegistrationFactory.ConsumerConfigs.TryGetValue(typeof(AutomatedConsumer).FullName!, out var config);
        Assert.True(result);
    }
}