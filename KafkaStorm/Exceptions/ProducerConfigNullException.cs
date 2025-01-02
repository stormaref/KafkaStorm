using System;

namespace KafkaStorm.Exceptions;

public class ProducerConfigNullException() : Exception("Producer Config is null");