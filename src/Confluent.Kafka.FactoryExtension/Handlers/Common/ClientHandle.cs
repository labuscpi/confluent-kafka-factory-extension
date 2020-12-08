using Confluent.Kafka.FactoryExtension.Interfaces.Handlers.Common;

namespace Confluent.Kafka.FactoryExtension.Handlers.Common
{
    public abstract class ClientHandle : IClientHandle
    {
        public string Topic { get; protected set; }
    }
}