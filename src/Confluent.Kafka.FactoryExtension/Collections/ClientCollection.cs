using System;
using System.Collections.Concurrent;
using System.Threading;
using Confluent.Kafka.FactoryExtension.Handlers.Common;

namespace Confluent.Kafka.FactoryExtension.Collections
{
    internal sealed class ClientCollection
    {
        public static ClientCollection Instance => LazyInstance.Value;

        private static readonly Lazy<ClientCollection> LazyInstance =
            new(() => new ClientCollection(), LazyThreadSafetyMode.ExecutionAndPublication);

        public readonly ConcurrentDictionary<string, Lazy<ClientHandle>> Handles;

        private ClientCollection()
            => Handles = new ConcurrentDictionary<string, Lazy<ClientHandle>>(StringComparer.Ordinal);
    }
}