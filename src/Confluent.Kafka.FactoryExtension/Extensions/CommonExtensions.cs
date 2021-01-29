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

using System.Collections.Generic;
using System.Net;
using Confluent.Kafka.FactoryExtension.Factories;
using Confluent.Kafka.FactoryExtension.Interfaces.Factories;
using Confluent.Kafka.FactoryExtension.Models.Settings.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Confluent.Kafka.FactoryExtension.Extensions
{
    internal static class CommonExtensions
    {
        public static void AddConsumerFactory(this IServiceCollection services, Dictionary<string, ConsumerSettings> collection)
        {
            if (collection == null)
                return;

            foreach (var (key, value) in collection)
            {
                services.AddOptions<ConsumerSettings>(key).Configure(settings =>
                {
                    settings.Topic = value.Topic;
                    settings.Separator = value.Separator;
                    settings.Config = value.Config;
                });

                services.PostConfigure<ConsumerSettings>(key, settings =>
                {
                    if (string.IsNullOrEmpty(settings.Config.ClientId))
                        settings.Config.ClientId = Dns.GetHostName();
                });
            }

            services.TryAddSingleton<IConsumerFactory, ConsumerFactory>();
        }

        public static void AddProducerFactory(this IServiceCollection services, Dictionary<string, ProducerSettings> collection)
        {
            if (collection == null)
                return;

            foreach (var (key, value) in collection)
            {
                services.AddOptions<ProducerSettings>(key).Configure(settings =>
                {
                    settings.Topic = value.Topic;
                    settings.Config = value.Config;
                });

                services.PostConfigure<ProducerSettings>(key, settings =>
                {
                    if (string.IsNullOrEmpty(settings.Config.ClientId))
                        settings.Config.ClientId = Dns.GetHostName();
                });
            }

            services.TryAddSingleton<IProducerFactory, ProducerFactory>();
        }
    }
}
