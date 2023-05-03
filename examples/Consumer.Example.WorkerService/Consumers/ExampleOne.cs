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
using Confluent.Kafka.FactoryExtensions.Interfaces.Factories;
using Consumer.Example.WorkerService.Consumers.Common;
using Microsoft.Extensions.Logging;

namespace Consumer.Example.WorkerService.Consumers
{
    public class ExampleOne : ProjectBackgroundService
    {
        private readonly IConsumerFactory _factory;
        private readonly ILogger<ExampleOne> _logger;

        public ExampleOne(IConsumerFactory factory, ILogger<ExampleOne> logger) : base("Constellation", logger)
        {
            _factory = factory;
            _logger = logger;

            SetupConsumer();
        }

        protected override void StartConsumerLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _factory.Create<long, string>(ConsumerName).Consume(cancellationToken);

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
            => _factory.Create<long, string>(ConsumerName).Builder
                .SetErrorHandler((_, error) => { _logger.LogDebug("{Reason}", error.Reason); })
                .SetLogHandler((_, message) => { _logger.LogDebug("{Message}", message.Message); });
    }
}
