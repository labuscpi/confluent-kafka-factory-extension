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

using System.IO;
using Confluent.Kafka.FactoryExtensions.Models;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Confluent.Kafka.FactoryExtensions.Tests.Helpers
{
    internal static class TestHelper
    {
        private static readonly string OutputPath = TestContext.CurrentContext.TestDirectory;

        public static IConfiguration GetIConfigurationRoot(string outputDirectory = null)
            => new ConfigurationBuilder()
                .SetBasePath(GetBasePath(outputDirectory))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

        public static IConfigurationSection GetConfigurationSection(string key, string outputDirectory = null)
            => GetIConfigurationRoot(outputDirectory).GetSection(key);

        public static KafkaSettings GetKafkaSettings(string outputDirectory = null)
        {
            var settings = new KafkaSettings();
            GetConfigurationSection(nameof(KafkaSettings), outputDirectory).Bind(settings);
            return settings;
        }

        private static string GetBasePath(string outputDirectory)
            => string.IsNullOrWhiteSpace(outputDirectory) ? OutputPath : Path.Join(OutputPath, outputDirectory);
    }
}
