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

using System;
using Confluent.Kafka.FactoryExtensions.Extensions;
using Confluent.Kafka.FactoryExtensions.Interfaces.Factories;
using Confluent.Kafka.FactoryExtensions.Models;
using Confluent.Kafka.FactoryExtensions.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Confluent.Kafka.FactoryExtensions.Tests.Extensions
{
    [TestFixture]
    public class ServiceCollectionShould
    {
        [Test]
        public void TryAddKafkaFactories_WithNUllKafkaSettings_ArgumentNullException()
            => Assert.That(() =>
            {
                var configuration = TestHelper.GetConfigurationSection("not_defined");
                new ServiceCollection().TryAddKafkaFactories(configuration);
            }, Throws.InstanceOf<ArgumentNullException>().With.Property("ParamName").EqualTo("settings"));

        [Test]
        public void TryAddKafkaFactories_WithConfiguration()
        {
            var configuration = TestHelper.GetIConfigurationRoot();
            var services = new ServiceCollection().TryAddKafkaFactories(configuration);

            ValidateRequiredService(services);
        }

        [Test]
        public void TryAddKafkaFactories_WithConfigurationSection()
        {
            var configuration = TestHelper.GetIConfigurationRoot().GetSection(KafkaSettings.Key);
            var services = new ServiceCollection().TryAddKafkaFactories(configuration);

            ValidateRequiredService(services);
        }

        private static void ValidateRequiredService(IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            Assert.That(sp.GetRequiredService<IOptions<KafkaSettings>>().Value, Is.InstanceOf<KafkaSettings>());
            Assert.That(sp.GetRequiredService<KafkaSettings>(), Is.InstanceOf<KafkaSettings>());
            Assert.That(sp.GetRequiredService<IConsumerFactory>(), Is.InstanceOf<IConsumerFactory>());
            Assert.That(sp.GetRequiredService<IProducerFactory>(), Is.InstanceOf<IProducerFactory>());
        }
    }
}
