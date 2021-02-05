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
using System.Collections.Concurrent;
using Confluent.Kafka;

namespace FactoryExtension.Services.Example.Collections
{
    public class ProducerMessageCollection
    {

        internal readonly ConcurrentDictionary<string, object> MessageCollection;
        internal readonly ConcurrentDictionary<string, DeliveryReport<Null, string>> ReportCollection;
        internal readonly ConcurrentQueue<string> Queue;

        public ProducerMessageCollection()
        {
            MessageCollection = new ConcurrentDictionary<string, object>();
            ReportCollection = new ConcurrentDictionary<string, DeliveryReport<Null, string>>();
            Queue = new ConcurrentQueue<string>();
        }

        public DeliveryReport<Null, string> EnqueueMessage(object message)
        {
            var key = Guid.NewGuid().ToString("D");
            MessageCollection.TryAdd(key, message);
            Queue.Enqueue(key);

            DeliveryReport<Null, string> report;
            do
            {
                if (ReportCollection.TryRemove(key, out report))
                    break;
            } while (true);

            return report;
        }
    }
}