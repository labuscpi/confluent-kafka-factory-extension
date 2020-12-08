using System;
using System.Collections.Generic;

namespace Confluent.Kafka.FactoryExtension.Builders
{
    public sealed class CustomConsumerBuilder<TKey, TValue> : ConsumerBuilder<TKey, TValue>
    {
        private IConsumer<TKey, TValue> _consumer;

        public CustomConsumerBuilder(IEnumerable<KeyValuePair<string, string>> config) : base(config)
        {
        }

        public override IConsumer<TKey, TValue> Build()
        {
            if (!IsValid())
                _consumer = base.Build();
            
            return _consumer;
        }

        private bool IsValid()
        {
            try
            {
                return _consumer != null && !string.IsNullOrEmpty(_consumer?.Name);
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