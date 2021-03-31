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
using Confluent.Kafka.FactoryExtension.Interfaces.Factories;
using Confluent.Kafka.FactoryExtension.Interfaces.Handlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consumer.Example.WorkerService.Consumers
{
    public class Constellation<TKey, TValue> : BackgroundService
    {
        private const string ConsumerName = "Constellation";
        private readonly IConsumerFactory _factory;
        private readonly ILogger<Constellation<TKey, TValue>> _logger;

        public Constellation(IConsumerFactory factory, ILogger<Constellation<TKey, TValue>> logger)
        {
            _factory = factory;
            _logger = logger;

            SetupConsumer();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            new Thread(() => StartConsumerLoop(stoppingToken)).Start();

            return Task.CompletedTask;
        }

        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _factory.Create<TKey, TValue>(ConsumerName).Consumer.Consume(cancellationToken);
                    _logger.LogInformation("{Key}: {Value}", cr.Message.Key, cr.Message.Value);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unexpected error: {Message}", e.Message);
                }
            }
        }

        /// <summary>
        ///  Create handle on name registered in Configuration, case sensitive
        /// Available Handler                         
        /// SetStatisticsHandler()                    
        /// SetOffsetsCommittedHandler()              
        /// SetPartitionsAssignedHandler()            
        /// SetPartitionsRevokedHandler()             
        /// SetOAuthBearerTokenRefreshHandler()       
        ///                                  
        /// Available Key and Value Deserializer Setup
        /// SetKeyDeserializer()                      
        /// SetValueDeserializer()     
        /// </summary>
        private void SetupConsumer()
            => _factory.Create<TKey, TValue>(ConsumerName).Builder
                .SetErrorHandler((_, error) =>
                {
                    _logger.LogDebug("{Reason}", error.Reason);
                })
                .SetLogHandler((_, message) =>
                {
                    _logger.LogDebug("{Message}", message.Message);
                });

    }
}