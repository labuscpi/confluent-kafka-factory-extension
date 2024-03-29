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
using Confluent.Kafka.FactoryExtensions.Models.Settings.Clients;

namespace Confluent.Kafka.FactoryExtensions.Models;

public sealed class KafkaSettings
{
    private const string OldValue = "Settings";
    public static string Key { get; } = nameof(KafkaSettings)
        .Replace(OldValue, "", StringComparison.CurrentCultureIgnoreCase);

    public Dictionary<string, ConsumerSettings> Consumers { get; set; }
    public Dictionary<string, ProducerSettings> Producers { get; set; }
}
