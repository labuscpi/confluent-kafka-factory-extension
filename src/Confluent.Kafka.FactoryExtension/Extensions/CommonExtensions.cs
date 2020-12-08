using System.Collections.Generic;
using System.Net;
using Confluent.Kafka.FactoryExtension.Factories;
using Confluent.Kafka.FactoryExtension.Interfaces.Factories;
using Confluent.Kafka.FactoryExtension.Models.Settings.Clients;
using Confluent.Kafka.FactoryExtension.Models.Settings.Clients.CommonSettings;
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
                services.ConfigureOptions<ConsumerConfig, ConsumerSettings>(key, value);

            services.TryAddSingleton<IConsumerFactory, ConsumerFactory>();
        }

        public static void AddProducerFactory(this IServiceCollection services, Dictionary<string, ProducerSettings> collection)
        {
            if (collection == null)
                return;

            foreach (var (key, value) in collection)
                services.ConfigureOptions<ProducerConfig, ProducerSettings>(key, value);

            services.TryAddSingleton<IProducerFactory, ProducerFactory>();
        }

        private static void ConfigureOptions<TConfig, TSettings>(this IServiceCollection services, string key, TSettings value)
            where TConfig : ClientConfig
            where TSettings : ClientSettings<TConfig>
        {
            services.AddOptions<TSettings>(key).Configure(settings =>
            {
                settings.Topic = value.Topic;
                settings.Config = value.Config;
            });

            services.PostConfigure<TSettings>(key, settings =>
            {
                if (string.IsNullOrEmpty(settings.Config.ClientId))
                    settings.Config.ClientId = Dns.GetHostName();
            });
        }
    }
}