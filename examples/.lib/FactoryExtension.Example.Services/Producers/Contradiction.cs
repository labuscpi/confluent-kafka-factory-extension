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

using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.FactoryExtension.Interfaces.Factories;
using Confluent.Kafka.FactoryExtension.Interfaces.Handlers;
using FactoryExtension.Example.Abstractions.Interfaces;
using FactoryExtension.Example.Utilities.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FactoryExtension.Example.Services.Producers
{
    public class Contradiction : BackgroundService
    {
        private readonly IProducerFactory _factory;
        private readonly IProduceHelper _collection;
        private readonly ILogger<Contradiction> _logger;

        public Contradiction(IProducerFactory factory, IProduceHelper collection, ILogger<Contradiction> logger)
        {
            _factory = factory;
            _collection = collection;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            new Thread(() => StartProducerLoop<Null, string>(stoppingToken)).Start();
 
            return Task.CompletedTask;
        }

        private void StartProducerLoop<TKey, TValue>(CancellationToken cancellationToken)
        {
            var handle = GetHandle<TKey, TValue>();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var (success, guid) = _collection.TryGetMessage<TKey, TValue>(out var message);
                    if (!success)
                        continue;

                    handle.Produce(message, report => { _collection.TryAddResult(guid, report); });
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    const string message = "Unexpected error: {0}";
                    _logger.LogError(e, message, e.GetMessage());
                }
            }
        }

        private IProducerHandle<TKey, TValue> GetHandle<TKey, TValue>()
        {
            // Create handle on name registered in Configuration, case sensitive
            var handle = _factory.Create<TKey, TValue>(nameof(Contradiction));

            // Optional Handler Action Setup
            handle.Builder
                .SetErrorHandler((_, error) => { _logger.LogError(error.Reason); })
                .SetLogHandler((_, message) => { _logger.LogInformation(message.Message); });

            // handle.Builder.SetKeySerializer();
            // handle.Builder.SetValueSerializer();

            // handle.Builder.SetDefaultPartitioner();
            // handle.Builder.SetPartitioner();
            // handle.Builder.SetErrorHandler();
            // handle.Builder.SetLogHandler();
            // handle.Builder.SetStatisticsHandler();
            // handle.Builder.SetOAuthBearerTokenRefreshHandler();

            return handle;
        }
    }
}