﻿#region Copyright
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
using Confluent.Kafka.FactoryExtension.Builders;
using Confluent.Kafka.FactoryExtension.Handlers.Common;
using Confluent.Kafka.FactoryExtension.Interfaces.Handlers;
using Confluent.Kafka.FactoryExtension.Models.Settings.Clients;

namespace Confluent.Kafka.FactoryExtension.Handlers
{
    internal sealed class ConsumerHandle<TKey, TValue> : ClientHandle, IConsumerHandle<TKey, TValue>, IDisposable
    {
        [ExcludeFromCodeCoverage] public CustomConsumerBuilder<TKey, TValue> Builder { get; }
        [ExcludeFromCodeCoverage] public IConsumer<TKey, TValue> Consumer => Builder.Build();

        private readonly string _separator;
        private readonly StringComparer _stringComparer = StringComparer.Ordinal;

        public ConsumerHandle(ConsumerSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            Topic = settings.Topic;
            _separator = settings.Separator;

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
            var topicList = new List<string>();
            if (string.IsNullOrWhiteSpace(_separator))
                topicList.Add(Topic);
            else
                topicList.AddRange(Topic.Split(_separator, StringSplitOptions.RemoveEmptyEntries).Distinct(_stringComparer));

            CheckIfSubscribed(topicList);

            return this;
        }

        [ExcludeFromCodeCoverage]
        private void CheckIfSubscribed(List<string> topicList)
        {
            if (topicList == null || !topicList.Any())
                throw new ArgumentNullException(nameof(topicList));
            
            topicList.Sort(_stringComparer);
            
            var consumerSubscription = Consumer.Subscription;
            if (consumerSubscription == null || !consumerSubscription.Any())
            {
                Consumer.Subscribe(topicList);
                return;
            }
            
            consumerSubscription.Sort(_stringComparer);

            if (topicList.SequenceEqual(consumerSubscription))
                return;
            
            Consumer.Unsubscribe();
            Consumer.Subscribe(topicList);
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
}