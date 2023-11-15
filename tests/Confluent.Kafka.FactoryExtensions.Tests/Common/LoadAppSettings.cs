#region Copyright
// Copyright 2020. labuscpi
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

using System.Linq;
using Confluent.Kafka.FactoryExtensions.Models;
using Confluent.Kafka.FactoryExtensions.Models.Settings.Clients;
using Confluent.Kafka.FactoryExtensions.Tests.Helpers;
using Microsoft.Extensions.Configuration;

namespace Confluent.Kafka.FactoryExtensions.Tests.Common
{
    public abstract class LoadAppSettings<T> where T : class
    {
        protected readonly KafkaSettings Settings = new();
        protected readonly string[] Names;

        protected LoadAppSettings()
        {
            TestHelper.GetConfigurationSection().Bind(Settings);
            Names = typeof(T) == typeof(ConsumerSettings)
                ? Settings.Consumers.Keys.ToArray()
                : Settings.Producers.Keys.ToArray();
        }
    }
}
