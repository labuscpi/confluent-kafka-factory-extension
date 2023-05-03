// Copyright 2023. labuscpi
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

using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.FactoryExtensions.Interfaces.Factories;
using Confluent.Kafka.FactoryExtensions.Interfaces.Handlers;
using FactoryExtension.Example.Common.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consumer.Example.WorkerService.Consumers.Common;

public abstract class ProjectBackgroundService<TKey, TValue> : BackgroundService
{
    protected readonly IConsumerHandle<TKey, TValue> Handle;
    private readonly ILogger _logger;

    protected ProjectBackgroundService(IConsumerFactory factory, string consumerName, ILogger logger)
    {
        Handle = factory.Create<TKey, TValue>(consumerName);
        _logger = logger;

        SetupConsumer();
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
        => Handle.Builder
            .SetErrorHandler(ErrorHandler())
            .SetLogHandler(LogHandler());

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

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        new Thread(() => StartConsumerLoop(stoppingToken)).Start();

        return Task.CompletedTask;
    }

    protected abstract void StartConsumerLoop(CancellationToken cancellationToken);

    protected void LogException(Exception e)
        => _logger.LogError(e, "{Message}", e.Message);
}
