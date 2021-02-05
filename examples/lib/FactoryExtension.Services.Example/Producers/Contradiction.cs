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
using FactoryExtension.Example.Abstractions.Extensions;
using FactoryExtension.Services.Example.Collections;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FactoryExtension.Services.Example.Producers
{
    public class Contradiction : BackgroundService
    {
        private readonly IProducerFactory _factory;
        private readonly ProducerMessageCollection _collection;
        private readonly ILogger<Contradiction> _logger;

        public Contradiction(IProducerFactory factory, ProducerMessageCollection collection, ILogger<Contradiction> logger)
        {
            _factory = factory;
            _collection = collection;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            new Thread(() => StartProducerLoop(stoppingToken)).Start();

            return Task.CompletedTask;
        }

        private void StartProducerLoop(CancellationToken cancellationToken)
        {
            var handle = GetHandle();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var key = TryGetMessage(out var message);
                    if (string.IsNullOrWhiteSpace(key))
                        continue;

                    handle.Produce(message, report => { _collection.ReportCollection.TryAdd(key, report); });
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ProduceException<Null, string> e)
                {
                    _logger.LogError(e, "Produce error: {0}", e.Error.Reason);

                    if (e.Error.IsFatal)
                        break;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unexpected error: {0}", e.Message);
                    break;
                }
            }
        }

        private string TryGetMessage(out Message<Null, string> message)
        {
            message = null;
        
            if (_collection.Queue.IsEmpty)
                return null;
            
            if (!_collection.Queue.TryDequeue(out var key))
                return null;
            
            if (!_collection.MessageCollection.TryRemove(key, out var value))
                return null;
        
            message = new Message<Null, string>
            {
                Value = value.SerializeObject()
            };
        
            return key;
        }

        private IProducerHandle<Null, string> GetHandle()
        {
            // Create handle on name registered in Configuration, case sensitive
            var handle = _factory.Create<Null, string>(nameof(Contradiction));

            // Optional Handler Action Setup
            handle.Builder
                .SetErrorHandler((_, error) => { _logger.LogError(error.Reason); })
                .SetLogHandler((_, message) => { _logger.LogInformation(message.Message); });

            // Available Handler
            // SetStatisticsHandler()
            // SetOffsetsCommittedHandler()
            // SetPartitionsAssignedHandler()
            // SetPartitionsRevokedHandler()
            // SetOAuthBearerTokenRefreshHandler()

            // Available Key and Value Deserializer Setup
            // SetKeyDeserializer()
            // SetValueDeserializer()

            return handle;
        }
    }
}