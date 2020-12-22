namespace Confluent.Kafka.FactoryExtension.Handlers.Common
{
    public abstract class ClientHandle
    {
        public string Topic { get; protected set; }
    }
}