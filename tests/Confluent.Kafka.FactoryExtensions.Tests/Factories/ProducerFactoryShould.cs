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
using Confluent.Kafka.FactoryExtensions.Factories;
using Confluent.Kafka.FactoryExtensions.Interfaces.Handlers;
using Confluent.Kafka.FactoryExtensions.Models.Settings.Clients;
using Confluent.Kafka.FactoryExtensions.Tests.Factories.Common;
using NUnit.Framework;

namespace Confluent.Kafka.FactoryExtensions.Tests.Factories
{
    [TestFixture]
    public class ProducerFactoryShould : FactoryBase<ProducerSettings>
    {
        private ProducerFactory _sut;
        
        [SetUp]
        public void Setup()
            => _sut = new ProducerFactory(Monitor);

        [Test]
        [TestCase(" ")]
        [TestCase("")]
        [TestCase(null)]
        public void NameIsNullOrWhiteSpace_ThrowArgumentNullException(string name)
            => Assert.That(() => _sut.Create<string, string>(name), Throws.InstanceOf<ArgumentNullException>()
                .With.Property("ParamName").EqualTo("name"));
        
        [Test]
        public void CreatHandle_WithValidName_ReturnsInstanceOfProducerHandle()
        {
            var handle = _sut.Create<string, string>(Names[0]);
            Assert.That(handle, Is.InstanceOf<IProducerHandle<string, string>>());
        }

        [Test]
        public void CreatHandle_InvalidName_ThrowsArgumentNullException()
            => Assert.That(() => _sut.Create<string, string>("random"), Throws.TypeOf<ArgumentNullException>());
        

        [Test]
        public void ConsecutiveCreateCalls_WithDifferentName_CreateDifferentHandle()
        {
            var handleX = _sut.Create<string, string>(Names[0]);
            var handleY = _sut.Create<string, string>(Names[0]);
            Assert.That(handleX, Is.SameAs(handleY));
        }
    }
}