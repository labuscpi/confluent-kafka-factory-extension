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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka.FactoryExtensions.Builders;
using Confluent.Kafka.FactoryExtensions.Handlers.Common;
using Confluent.Kafka.FactoryExtensions.Interfaces.Handlers;
using Confluent.Kafka.FactoryExtensions.Models.Settings.Clients;

namespace Confluent.Kafka.FactoryExtensions.Handlers;

internal sealed class ConsumerHandle<TKey, TValue> : ClientHandle, IConsumerHandle<TKey, TValue>, IDisposable
{
    [ExcludeFromCodeCoverage] public string Separator { get; }
    [ExcludeFromCodeCoverage] public CustomConsumerBuilder<TKey, TValue> Builder { get; }
    [ExcludeFromCodeCoverage] public IConsumer<TKey, TValue> Consumer => Builder.Build();

    private readonly StringComparer _stringComparer = StringComparer.Ordinal;

    public ConsumerHandle(ConsumerSettings settings)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        Topic = settings.Topic;
        Separator = settings.Separator;

        var config = new ConsumerConfig();
        foreach (var (key, value) in settings.Config.Where(x => !string.IsNullOrEmpty(x.Value)))
            config.Set(key, value);

        Builder = new CustomConsumerBuilder<TKey, TValue>(config);
    }

    [ExcludeFromCodeCoverage]
    public ConsumeResult<TKey, TValue> Consume(int millisecondsTimeout)
        => Subscribe().Consumer.Consume(millisecondsTimeout);

    [ExcludeFromCodeCoverage]
    public ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken = default)
        => Subscribe().Consumer.Consume(cancellationToken);

    [ExcludeFromCodeCoverage]
    public ConsumeResult<TKey, TValue> Consume(TimeSpan timeout)
        => Subscribe().Consumer.Consume(timeout);

    [ExcludeFromCodeCoverage]
    private ConsumerHandle<TKey, TValue> Subscribe()
    {
        var subscription = CreateTopics();

        var consumerSubscription = Consumer.Subscription;
        if (consumerSubscription == null || !consumerSubscription.Any())
        {
            Consumer.Subscribe(subscription);
            return this;
        }

        consumerSubscription.Sort(_stringComparer);

        if (subscription.SequenceEqual(consumerSubscription))
            return this;

        Consumer.Unsubscribe();
        Consumer.Subscribe(subscription);

        return this;
    }

    [ExcludeFromCodeCoverage]
    public List<string> CreateTopics()
    {
        var topics = string.IsNullOrWhiteSpace(Separator)
            ? new List<string> { Topic }
            : Topic
                .Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                .Distinct(_stringComparer)
                .ToList();

        if (topics == null || !topics.Any())
            throw new ArgumentNullException(nameof(Topic));

        topics.Sort(_stringComparer);

        return topics;
    }

    [ExcludeFromCodeCoverage]
    public void Dispose()
    {
        try
        {
            Consumer?.Close();
            Consumer?.Dispose();
        }
        catch (Exception e)
        {
            if (e is TaskCanceledException)
                return;

            throw;
        }
    }
}
