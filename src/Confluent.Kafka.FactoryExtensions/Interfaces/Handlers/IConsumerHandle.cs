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
using Confluent.Kafka.FactoryExtensions.Builders;
using Confluent.Kafka.FactoryExtensions.Interfaces.Handlers.Common;

namespace Confluent.Kafka.FactoryExtensions.Interfaces.Handlers;

public interface IConsumerHandle<TKey, TValue> : IClientHandle
{
    string Separator { get;  }
    CustomConsumerBuilder<TKey, TValue> Builder { get; }
    IConsumer<TKey, TValue> Consumer { get; }

    ConsumeResult<TKey, TValue> Consume(int millisecondsTimeout);
    ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken = default);
    ConsumeResult<TKey, TValue> Consume(TimeSpan timeout);
}