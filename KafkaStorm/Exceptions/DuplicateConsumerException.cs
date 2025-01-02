using System;

namespace KafkaStorm.Exceptions;

public class DuplicateConsumerException(string name) : Exception($"Consumer added more than once ({name})");