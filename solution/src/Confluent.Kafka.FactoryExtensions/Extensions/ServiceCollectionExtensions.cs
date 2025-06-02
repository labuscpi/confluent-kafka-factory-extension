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
using System.Linq;
using Confluent.Kafka.FactoryExtensions.Factories;
using Confluent.Kafka.FactoryExtensions.Models;
using Confluent.Kafka.FactoryExtensions.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Confluent.Kafka.FactoryExtensions.Extensions;

public static class ServiceCollectionExtensions
{
    private static IServiceCollection TryAddKafkaFactories(this IServiceCollection services, KafkaSettings settings)
    {
        if (settings == null || settings.Consumers == null && settings.Producers == null)
            throw new ArgumentNullException(nameof(settings));

        new ClientSettingsValidator().ValidateAndThrow(settings);

        services.TryAddSingleton(sp => sp.GetRequiredService<IOptions<KafkaSettings>>().Value);

        if (settings.Consumers != null)
            services.AddConsumerFactory(settings.Consumers);

        if (settings.Producers != null)
            services.AddProducerFactory(settings.Producers);

        return services;
    }

    public static IServiceCollection TryAddKafkaFactories(this IServiceCollection services, IConfiguration configuration, string key = null)
    {
        key ??= KafkaSettings.Key;

        var section = configuration.GetSection(key);

        return services.TryAddKafkaFactories(section);
    }

    public static IServiceCollection TryAddKafkaFactories(this IServiceCollection services, IConfigurationSection section)
        => services
            .Configure<KafkaSettings>(section, o => o.BindNonPublicProperties = true)
            .TryAddKafkaFactories(section.Get<KafkaSettings>());
}
