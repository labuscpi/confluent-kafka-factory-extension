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

using Confluent.Kafka.FactoryExtensions.Extensions;
using Confluent.Kafka.FactoryExtensions.Tests.Common;
using Confluent.Kafka.FactoryExtensions.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Confluent.Kafka.FactoryExtensions.Tests.Factories.Common
{
    public abstract class FactoryBase<T> : LoadAppSettings<T> where T : class
    {
        protected IOptionsMonitor<T> Monitor;

        [OneTimeSetUp]
        protected void OneTimeSetUp()
        {
            var services = new ServiceCollection();
            services.TryAddKafkaFactories(TestHelper.GetIConfigurationRoot());

            Monitor = services.BuildServiceProvider().GetRequiredService<IOptionsMonitor<T>>();
        }
    }
}
