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
using Confluent.Kafka;
using Confluent.Kafka.FactoryExtensions.Interfaces.Factories;
using Confluent.Kafka.FactoryExtensions.Interfaces.Handlers;
using Consumer.Example.WorkerService.Consumers.Common;
using FactoryExtension.Example.Common.Extensions;
using Microsoft.Extensions.Logging;

namespace Consumer.Example.WorkerService.Consumers
{
    public class ExampleTwo<TKey, TValue> : ProjectBackgroundService
    {
        private readonly IConsumerHandle<TKey, TValue> _handle;
        private readonly ILogger<ExampleTwo<TKey, TValue>> _logger;

        public ExampleTwo(IConsumerFactory factory, ILogger<ExampleTwo<TKey, TValue>> logger) : base("Constellation", logger)
        {
            _handle = factory.Create<TKey, TValue>(ConsumerName);
            _logger = logger;
        }

        protected override void StartConsumerLoop(CancellationToken cancellationToken)
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

                    HandleMessage(cr.Message);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    LogException(e);
                }
            }
        }

        private Action<IConsumer<TKey, TValue>, Error> ErrorHandler()
            => (_, error) =>
            {
                var e = new KafkaException(error);
                LogException(e);
            };

        private Action<IConsumer<TKey, TValue>, LogMessage> LogHandler()
            => (_, logMessage) =>
            {
                _logger.LogInformation("{Message}", logMessage.Message);
            };
    }
}
