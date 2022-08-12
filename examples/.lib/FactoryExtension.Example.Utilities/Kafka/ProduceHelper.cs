#region Copyright

// Copyright 2021. labuscpi
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.FactoryExtensions.Interfaces.Factories;
using FactoryExtension.Example.Utilities.Interfaces;
using Microsoft.Extensions.Logging;

namespace FactoryExtension.Example.Utilities.Kafka
{
    public class ProduceHelper<TKey, TValue> : IProduceHelper<TKey, TValue>
    {
        private const string ProducerName = "Contradiction";
        
        private readonly IProducerFactory _factory;
        private readonly ILogger<ProduceHelper<TKey, TValue>> _logger;

        public ProduceHelper(IProducerFactory factory, ILogger<ProduceHelper<TKey, TValue>> logger)
        {
            _factory = factory;
            _logger = logger;
            
            ConfigureHandle();
        }

        public Task<DeliveryResult<TKey, TValue>> SendMessageAsync(TValue value)
        {
            var message = new Message<TKey, TValue>
            {
                Value = value
            };

            return Produce(message);
        }

        public Task<DeliveryResult<TKey, TValue>> SendMessageAsync(TKey key, TValue value)
        {
            var message = new Message<TKey, TValue>
            {
                Key = key,
                Value = value
            };

            return Produce(message);
        }
        
        private Task<DeliveryResult<TKey, TValue>> Produce(Message<TKey, TValue> message)
            => _factory.Create<TKey, TValue>(ProducerName).ProduceAsync(message);
        
        private void ConfigureHandle()
            => _factory.Create<TKey, TValue>(ProducerName)
                .Builder
                .SetErrorHandler((_, e) =>
                {
                    _logger.LogDebug("[{Producer}::Error Handler::{Reason}]", ProducerName, e.Reason);
                })
                .SetLogHandler((_, m) =>
                {
                    _logger.LogDebug("[{Producer}::Log Handler::{Message}]", ProducerName, m.Message);
                });
    }
}