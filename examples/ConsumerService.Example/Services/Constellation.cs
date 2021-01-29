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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsumerService.Example.Services
{
    public class Constellation : BackgroundService
    {
        private readonly IConsumerFactory _factory;
        private readonly ILogger<Constellation> _logger;

        public Constellation(IConsumerFactory factory, ILogger<Constellation> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            new Thread(() => StartConsumerLoop<string, string>(stoppingToken)).Start();

            return Task.CompletedTask;
        }

        private void StartConsumerLoop<TKey, TValue>(CancellationToken cancellationToken)
        {
            var handle = GetHandle<TKey, TValue>();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = handle.Consume(cancellationToken);
                    Log(LogLevel.Information, "{0}: {1}", cr.Message.Key, cr.Message.Value);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    Log(LogLevel.Error, "Consume error: {0}", e.Error.Reason);

                    if (e.Error.IsFatal)
                        break;
                }
                catch (Exception e)
                {
                    Log(LogLevel.Error, "Unexpected error: {0}", e.Message);
                    break;
                }
            }
        }

        private IConsumerHandle<TKey, TValue> GetHandle<TKey, TValue>()
        {
            // Create handle on name registered in Configuration. Case sensitive!!
            var handle = _factory.Create<TKey, TValue>(nameof(Constellation));

            // Optional builder customization
            // handle.Builder
            //     .SetErrorHandler((_, error) => { Log(LogLevel.Error, error.Reason); })
            //     .SetLogHandler((_, message) => { Log(LogLevel.Information, message.Message); })
            //     .SetStatisticsHandler((_, s) => { })
            //     .SetOffsetsCommittedHandler((_, offsets) => { })
            //     .SetPartitionsAssignedHandler((_, list) => { })
            //     .SetPartitionsRevokedHandler((_, list) => { })
            //     .SetOAuthBearerTokenRefreshHandler((_, s) => { })
            //     .SetKeyDeserializer()
            //     .SetValueDeserializer();

            return handle;
        }

        private void Log(LogLevel logLevel, string format, params object[] args)
            => _logger.Log(logLevel, format, args);
    }
}