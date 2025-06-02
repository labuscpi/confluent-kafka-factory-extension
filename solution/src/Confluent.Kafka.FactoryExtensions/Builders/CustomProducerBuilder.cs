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

namespace Confluent.Kafka.FactoryExtensions.Builders;

[ExcludeFromCodeCoverage]
public sealed class CustomProducerBuilder<TKey, TValue> : ProducerBuilder<TKey, TValue>
{
    private IProducer<TKey, TValue> _producer;

    public CustomProducerBuilder(IEnumerable<KeyValuePair<string, string>> config) : base(config)
    {
    }

    public override IProducer<TKey, TValue> Build()
    {
        if (!IsValid())
            _producer = base.Build();

        return _producer;
    }

    private bool IsValid()
    {
        try
        {
            return _producer != null && !string.IsNullOrEmpty(_producer?.Name);
        }
        catch (Exception ex)
        {
            switch (ex)
            {
                case NullReferenceException _:
                case ObjectDisposedException _:
                    return false;
                default:
                    throw;
            }
        }
    }
}
