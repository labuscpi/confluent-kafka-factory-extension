using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using Confluent.Kafka.FactoryExtension.Handlers.Common;
using Confluent.Kafka.FactoryExtension.Models.Settings.Clients;
using Confluent.Kafka.FactoryExtension.Validators;
using Microsoft.Extensions.Options;

namespace Confluent.Kafka.FactoryExtension.Factories.Common
{
    public abstract class ClientFactory<TSettings>
    {
        private const string Separator = "::";

        private string _keyPrefix;
        private readonly IOptionsMonitor<TSettings> _optionsMonitor;

        protected ClientFactory(IOptionsMonitor<TSettings> optionsMonitor)
            => _optionsMonitor = optionsMonitor;

        protected Func<string, Lazy<ClientHandle>> HandleFactory<TKey, TValue>()
            => name => new Lazy<ClientHandle>(() => CreateHandle<TKey, TValue>(name), LazyThreadSafetyMode.ExecutionAndPublication);

        protected abstract ClientHandle CreateHandle<TKey, TValue>(string name);

        protected void SetKeyPrefix(Type type)
        {
            if (!string.IsNullOrWhiteSpace(_keyPrefix))
                return;
            
            var identifier = Guid.NewGuid().ToString("N");
            
            _keyPrefix = new StringBuilder(type.Name).Append(Separator).Append(identifier).Append(Separator).ToString();
        }

        protected string GetClientHandleKey(string name)
            => new StringBuilder(_keyPrefix).Append(name).ToString();

        [ExcludeFromCodeCoverage]
        protected TSettings GetSettings(string name)
        {
            var settings = _optionsMonitor.Get(name.Replace(_keyPrefix, "", StringComparison.OrdinalIgnoreCase));
            return settings switch
            {
                ConsumerSettings consumerSettings => new ConsumerSettingsValidator().Validate(consumerSettings).IsValid ? settings : default,
                ProducerSettings producerSettings => new ProducerSettingsValidator().Validate(producerSettings).IsValid ? settings : default,
                _ => throw new NotSupportedException()
            };
        }
    }
}