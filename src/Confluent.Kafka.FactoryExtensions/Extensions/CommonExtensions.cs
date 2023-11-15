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
using Confluent.Kafka.FactoryExtensions.Factories;
using Confluent.Kafka.FactoryExtensions.Interfaces.Factories;
using Confluent.Kafka.FactoryExtensions.Models.Settings.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Confluent.Kafka.FactoryExtensions.Extensions;

internal static class CommonExtensions
{
    public static void AddConsumerFactory<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory)
        where TService : class
    {
        services.TryAdd(ServiceDescriptor.Singleton<TService>(implementationFactory));
    }

    public static void AddConsumerFactory(this IServiceCollection services, Dictionary<string, ConsumerSettings> collection)
    {
        if (collection == null)
            return;

        foreach (var (key, consumerSettings) in collection)
        {
            services.AddOptions<ConsumerSettings>(key).Configure(cs =>
            {
                cs.Topic = consumerSettings.Topic;
                cs.Separator = consumerSettings.Separator;
                cs.Config = consumerSettings.Config;
            });
        }

        services.TryAddSingleton<IConsumerFactory, ConsumerFactory>();
    }

    public static void AddProducerFactory(this IServiceCollection services, Dictionary<string, ProducerSettings> collection)
    {
        if (collection == null)
            return;

        foreach (var (key, producerSettings) in collection)
        {
            services.AddOptions<ProducerSettings>(key).Configure(ps =>
            {
                ps.Topic = producerSettings.Topic;
                ps.Config = producerSettings.Config;
            });
        }

        services.TryAddSingleton<IProducerFactory, ProducerFactory>();
    }
}
