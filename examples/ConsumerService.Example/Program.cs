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

using Confluent.Kafka.FactoryExtension.Extensions;
using Confluent.Kafka.FactoryExtension.Models;
using FactoryExtension.Services.Example.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsumerService.Example
{
    public class Program
    {
        public static void Main(string[] args)
            => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((_, configApp) =>
                {
                    configApp.AddJsonFile($"{nameof(KafkaSettings)}.json", false, false);
                    configApp.AddJsonFile("Secrets.json", true, false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration.GetSection(nameof(KafkaSettings));
                    services.TryAddKafkaFactories(configuration);

                    services.AddHostedService<Constellation>();
                    // services.AddHostedService<Qualification>();
                });
    }
}