using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Confluent.Kafka.FactoryExtension.Builders
{
    [ExcludeFromCodeCoverage]
    public sealed class CustomProducerBuilder<TKey, TValue> : ProducerBuilder<TKey, TValue>
    {
        private IProducer<TKey, TValue> _producer;

        public CustomProducerBuilder(IEnumerable<KeyValuePair<string, string>> config) : base(config)
        {
        }

        public override IProducer<TKey, TValue> Build()
        {
            if (!IsValid())
                _producer = base.Build();

            return _producer;
        }

        private bool IsValid()
        {
            try
            {
                return _producer != null && !string.IsNullOrEmpty(_producer?.Name);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case NullReferenceException _:
                    case ObjectDisposedException _:
                        return false;
                    default:
                        throw;
                }
            }
        }
    }
}
