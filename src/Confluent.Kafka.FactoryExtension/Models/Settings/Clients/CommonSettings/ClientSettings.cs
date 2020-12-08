namespace Confluent.Kafka.FactoryExtension.Models.Settings.Clients.CommonSettings
{
    public abstract class ClientSettings<T> where T : Config
    {
        public string Topic { get; set; }
        public T Config { get; set; }
    }
}