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
using FactoryExtension.Example.Common.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consumer.Example.WorkerService.Consumers.Common;

public abstract class ProjectBackgroundService : BackgroundService
{
    protected readonly string ConsumerName;
    private readonly ILogger _logger;

    protected ProjectBackgroundService(string consumerName, ILogger logger)
    {
        ConsumerName = consumerName;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        new Thread(() => StartConsumerLoop(stoppingToken)).Start();

        return Task.CompletedTask;
    }

    protected abstract void StartConsumerLoop(CancellationToken cancellationToken);

    protected void HandleMessage<TKey, TValue>(Message<TKey, TValue> message)
    {
        try
        {
            if (message.Value is string)
                _logger.LogInformation("{Key}: {Value}", message.Key, message.Value);
            else
                _logger.LogInformation("{Key}: {Value}", message.Key, message.Value.SerializeObject());
        }
        catch (Exception e)
        {
            LogException(e);
        }
    }

    protected void LogException(Exception e)
        => _logger.LogError(e, "{Message}", e.Message);
}
