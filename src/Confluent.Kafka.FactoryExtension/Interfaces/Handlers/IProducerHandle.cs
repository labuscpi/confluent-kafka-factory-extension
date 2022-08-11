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
using Confluent.Kafka.FactoryExtension.Builders;
using Confluent.Kafka.FactoryExtension.Interfaces.Handlers.Common;

namespace Confluent.Kafka.FactoryExtension.Interfaces.Handlers;

public interface IProducerHandle<TKey, TValue> : IClientHandle
{
    CustomProducerBuilder<TKey, TValue> Builder { get; }
    IProducer<TKey, TValue> Producer { get; }

    Message<TKey, TValue> CreateMessage(TKey key, TValue value, Headers headers = null);
    void Produce(Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>> deliveryHandler = null);
    void Produce(Partition partition, Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>> deliveryHandler = null);
    Task<DeliveryResult<TKey, TValue>> ProduceAsync(Message<TKey, TValue> message, CancellationToken cancellationToken = default);
    Task<DeliveryResult<TKey, TValue>> ProduceAsync(Partition partition, Message<TKey, TValue> message, CancellationToken cancellationToken = default);
}