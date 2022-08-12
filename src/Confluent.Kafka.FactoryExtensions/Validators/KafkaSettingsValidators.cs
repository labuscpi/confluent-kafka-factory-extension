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
using Confluent.Kafka.FactoryExtensions.Models;
using Confluent.Kafka.FactoryExtensions.Models.Settings.Clients;
using FluentValidation;

namespace Confluent.Kafka.FactoryExtensions.Validators;

public class ClientSettingsValidator : AbstractValidator<KafkaSettings>
{
    public ClientSettingsValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
            
        RuleForEach(x => x.Consumers).SetValidator(new ConsumersValidator());
        RuleForEach(x => x.Producers).SetValidator(new ProducersValidator());
    }
}

public class ConsumersValidator : AbstractValidator<KeyValuePair<string, ConsumerSettings>>
{
    public ConsumersValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
            
        RuleFor(x => x.Key).NotEmpty();
        RuleFor(x => x.Value).SetValidator(new ConsumerSettingsValidator());
    }
}

public class ConsumerSettingsValidator : AbstractValidator<ConsumerSettings>
{
    public ConsumerSettingsValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
            
        RuleFor(x => x.Topic).NotEmpty();
        RuleFor(x => x.Config).NotEmpty();
        When(x => x.Config != null,
            () => RuleFor(x => x.Config.BootstrapServers).NotEmpty());
    }
}

public class ProducersValidator : AbstractValidator<KeyValuePair<string, ProducerSettings>>
{
    public ProducersValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
            
        RuleFor(x => x.Key).NotEmpty();
        RuleFor(x => x.Value).SetValidator(new ProducerSettingsValidator());
    }
}

public class ProducerSettingsValidator : AbstractValidator<ProducerSettings>
{
    public ProducerSettingsValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
            
        RuleFor(x => x.Topic).NotEmpty();
        RuleFor(x => x.Config).NotEmpty();
        When(x => x.Config != null,
            () => RuleFor(x => x.Config.BootstrapServers).NotEmpty());
    }
}