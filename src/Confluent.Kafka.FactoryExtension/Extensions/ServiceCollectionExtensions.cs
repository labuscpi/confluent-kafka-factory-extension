using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka.FactoryExtension.Models;
using Confluent.Kafka.FactoryExtension.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Confluent.Kafka.FactoryExtension.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static bool TryAddKafkaFactories(this IServiceCollection services, KafkaSettings settings)
        {
            if (settings == null || settings.Consumers == null && settings.Producers == null)
                return false;

            new ClientSettingsValidator().ValidateAndThrow(settings);
            
            services.TryAddSingleton(settings);

            services.AddConsumerFactory(settings.Consumers);

            services.AddProducerFactory(settings.Producers);

            return true;
        }

        public static bool TryAddKafkaFactories(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration is IConfigurationSection section)
                return services.TryAddKafkaFactories(section);

            return services.TryAddKafkaFactories(configuration.GetChildren());
        }

        public static bool TryAddKafkaFactories(this IServiceCollection services, IConfigurationSection section)
            => services.TryAddKafkaFactories(section.Get<KafkaSettings>());

        public static bool TryAddKafkaFactories(this IServiceCollection services, IEnumerable<IConfigurationSection> sections)
            => (from section in sections
                select section.Get<KafkaSettings>()
                into settings
                where settings != null && (settings.Consumers != null || settings.Producers != null)
                select services.TryAddKafkaFactories(settings)).FirstOrDefault();
    }
}