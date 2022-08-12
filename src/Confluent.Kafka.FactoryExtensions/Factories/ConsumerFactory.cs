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
using Confluent.Kafka.FactoryExtensions.Collections;
using Confluent.Kafka.FactoryExtensions.Factories.Common;
using Confluent.Kafka.FactoryExtensions.Handlers;
using Confluent.Kafka.FactoryExtensions.Handlers.Common;
using Confluent.Kafka.FactoryExtensions.Interfaces.Factories;
using Confluent.Kafka.FactoryExtensions.Interfaces.Handlers;
using Confluent.Kafka.FactoryExtensions.Models.Settings.Clients;
using Microsoft.Extensions.Options;

namespace Confluent.Kafka.FactoryExtensions.Factories;

public sealed class ConsumerFactory : ClientFactory<ConsumerSettings>, IConsumerFactory
{
    public ConsumerFactory(IOptionsMonitor<ConsumerSettings> optionsMonitor) : base(optionsMonitor)
    {
    }

    public IConsumerHandle<TKey, TValue> Create<TKey, TValue>(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        SetKeyPrefix(GetType());

        return (IConsumerHandle<TKey, TValue>) ClientCollection.Instance.Handles
            .GetOrAdd(GetClientHandleKey(name), HandleFactory<TKey, TValue>()).Value;
    }

    protected override ClientHandle CreateHandle<TKey, TValue>(string name)
        => new ConsumerHandle<TKey, TValue>(GetSettings(name));
}