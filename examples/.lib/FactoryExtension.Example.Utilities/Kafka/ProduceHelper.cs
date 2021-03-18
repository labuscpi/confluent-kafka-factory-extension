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
using System.Threading;
using Confluent.Kafka;
using FactoryExtension.Example.Abstractions.Interfaces;

namespace FactoryExtension.Example.Utilities.Kafka
{
    public class ProduceHelper : IProduceHelper, IProduceHelperIngest
    {
        private readonly ConcurrentDictionary<Guid, object> _messageCollection;
        private readonly ConcurrentDictionary<Guid, object> _reportCollection;
        private readonly ConcurrentQueue<Guid> _queue;

        public ProduceHelper()
        {
            _messageCollection = new ConcurrentDictionary<Guid, object>();
            _reportCollection = new ConcurrentDictionary<Guid, object>();
            _queue = new ConcurrentQueue<Guid>();
        }

        public string EnqueueMessage<TKey, TValue>(Message<TKey, TValue> message, bool enableReport)
        {
            var guid = Guid.NewGuid();

            _messageCollection.TryAdd(guid, message);
            if (enableReport)
                _reportCollection.TryAdd(guid, null);
            _queue.Enqueue(guid);

            return GetKey(guid);
        }

        public DeliveryResult<TKey, TValue> GetDeliveryResult<TKey, TValue>(string key, CancellationToken cancellationToken = default)
        {
            var guid = GetGuid(key);

            CheckToken(ref cancellationToken);
            
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if(!_reportCollection.TryGetValue(guid, out var value))
                        continue;
                        
                    if (value != null)
                        break;
                }

                cancellationToken.ThrowIfCancellationRequested();

                _reportCollection.TryRemove(guid, out var resultObject);

                return (DeliveryResult<TKey, TValue>) resultObject;
            }
            catch (OperationCanceledException)
            {
                _reportCollection.TryRemove(guid, out _);
                throw;
            }
        }

        private static void CheckToken(ref CancellationToken cancellationToken)
        {
            if (!cancellationToken.CanBeCanceled)
                cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(15)).Token;
        }

        public (bool success, Guid guid) TryGetMessage<TKey, TValue>(out Message<TKey, TValue> data)
        {
            data = null;

            if (_queue.IsEmpty)
                return (false, Guid.Empty);

            if (!_queue.TryDequeue(out var guid))
                return (false, Guid.Empty);

            if (!_messageCollection.TryRemove(guid, out var value))
                return (false, guid);

            if (value is not Message<TKey, TValue>)
                return (false, guid);

            data = (Message<TKey, TValue>) value;

            return (true, guid);
        }

        public bool TryAddResult(Guid guid, object newValue)
            => _reportCollection.TryGetValue(guid, out var comparisonValue) && _reportCollection.TryUpdate(guid, newValue, comparisonValue);

        private static string GetKey(Guid guid)
            => guid.ToString("N");

        private static Guid GetGuid(string key)
            => Guid.Parse(key);
    }
}