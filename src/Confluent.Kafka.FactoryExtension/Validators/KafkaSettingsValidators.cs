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

using System.Collections.Generic;
using Confluent.Kafka.FactoryExtension.Models;
using Confluent.Kafka.FactoryExtension.Models.Settings.Clients;
using FluentValidation;

namespace Confluent.Kafka.FactoryExtension.Validators
{
    public class ClientSettingsValidator : AbstractValidator<KafkaSettings>
    {
        public ClientSettingsValidator()
        {
            RuleForEach(x => x.Consumers).SetValidator(new ConsumersValidator());
            RuleForEach(x => x.Producers).SetValidator(new ProducersValidator());
        }
    }

    public class ConsumersValidator : AbstractValidator<KeyValuePair<string, ConsumerSettings>>
    {
        public ConsumersValidator()
        {
            RuleFor(x => x.Key).NotEmpty();
            RuleFor(x => x.Value).SetValidator(new ConsumerSettingsValidator());
        }
    }

    public class ConsumerSettingsValidator : AbstractValidator<ConsumerSettings>
    {
        public ConsumerSettingsValidator()
        {
            RuleFor(x => x.Topic).NotEmpty();
            RuleFor(x => x.Config).NotEmpty();
            RuleFor(x => x.Config.BootstrapServers).NotEmpty();
        }
    }

    public class ProducersValidator : AbstractValidator<KeyValuePair<string, ProducerSettings>>
    {
        public ProducersValidator()
        {
            RuleFor(x => x.Key).NotEmpty();
            RuleFor(x => x.Value).SetValidator(new ProducerSettingsValidator());
        }
    }

    public class ProducerSettingsValidator : AbstractValidator<ProducerSettings>
    {
        public ProducerSettingsValidator()
        {
            RuleFor(x => x.Topic).NotEmpty();
            RuleFor(x => x.Config).NotEmpty();
            RuleFor(x => x.Config.BootstrapServers).NotEmpty();
        }
    }
}