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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.FactoryExtensions.Interfaces.Factories;
using Confluent.Kafka.FactoryExtensions.Interfaces.Handlers;
using FactoryExtension.Example.Common.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consumer.Example.WorkerService.Consumers
{
    public class Qualification<TKey, TValue> : BackgroundService
    {
        private readonly IConsumerHandle<TKey, TValue> _handle;
        private readonly ILogger<Qualification<TKey, TValue>> _logger;

        public Qualification(IConsumerFactory factory, ILogger<Qualification<TKey, TValue>> logger)
        {
            _handle = factory.Create<TKey, TValue>(nameof(Qualification<TKey, TValue>));
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            new Thread(() => StartConsumerLoop(stoppingToken)).Start();

            return Task.CompletedTask;
        }

        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            using var consumer = _handle.Builder
                .SetErrorHandler(ErrorHandler())
                .SetLogHandler(LogHandler())
                .Build();

            consumer.Subscribe(_handle.CreateTopics());

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(cancellationToken);
                    _logger.LogInformation("{Key}: {Value}", cr.Message.Key, cr.Message.Value);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unexpected error: {Message}", e.GetMessage());
                }
            }
        }

        private Action<IConsumer<TKey, TValue>, Error> ErrorHandler()
            => (_, e) =>
            {
                var ex = new KafkaException(e);
                _logger.LogError(ex, "{Message}",ex.Message);
            };

        private Action<IConsumer<TKey, TValue>, LogMessage> LogHandler()
            => (_, logMessage) =>
            {
                _logger.LogInformation("{@Message}", logMessage);
            };

        private IEnumerable<string> Topics()
            => string.IsNullOrWhiteSpace(_handle.Separator)
                ? new List<string> {_handle.Topic}
                : _handle.Topic
                    .Split(_handle.Separator, StringSplitOptions.RemoveEmptyEntries)
                    .Distinct(StringComparer.Ordinal)
                    .ToList();

        // Optional builder customization
        private IConsumer<TKey, TValue> BuildConsumer()
            => _handle.Builder
                .SetErrorHandler((_, error) =>
                {
                    _logger.LogDebug("{Reason}", error.Reason);
                })
                .SetLogHandler((_, message) =>
                {
                    _logger.LogDebug("{Message}", message.Message);
                })
                .Build();
    }
}
