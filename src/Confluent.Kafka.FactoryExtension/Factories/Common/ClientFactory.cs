using System;
using System.Text;
using System.Threading;
using Confluent.Kafka.FactoryExtension.Collections;
using Confluent.Kafka.FactoryExtension.Handlers.Common;

namespace Confluent.Kafka.FactoryExtension.Factories.Common
{
    public abstract class ClientFactory
    {
        private const string Separator = "::";
        protected string KeyPrefix { get; private set; }

        internal static ClientCollection ClientCollection => ClientCollection.Instance;

        protected Func<string, Lazy<ClientHandle>> ActiveHandleFactory<TKey, TValue>()
            => name => new Lazy<ClientHandle>(() => CreateHandle<TKey, TValue>(name), LazyThreadSafetyMode.ExecutionAndPublication);

        protected abstract ClientHandle CreateHandle<TKey, TValue>(string name);

        protected void SetKeyPrefix(string value)
        {
            if (!string.IsNullOrEmpty(KeyPrefix))
                return;
            
            var identifier = Guid.NewGuid().ToString("N");
            
            KeyPrefix = new StringBuilder(value).Append(Separator).Append(identifier).Append(Separator).ToString();
        }

        protected string GetClientHandleKey(string name)
            => new StringBuilder(KeyPrefix).Append(name).ToString();
    }
}