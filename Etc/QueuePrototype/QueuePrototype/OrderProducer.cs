// Producer 类（使用一个辅助线程）

// 将项异步添加到队列中，共添加 20 个项。

using System;
using System.Collections;
using System.Collections.Generic;

namespace QueuePrototype
{
    public class OrderProducer
    {
        public int ProduceCount { get; private set; }

        public OrderProducer(Queue<Order> q, SyncEvents e)
        {

            _queue = q;

            _syncEvents = e;

        }

        public void EnqueueOrder(Order order)
        {
            lock (((ICollection)_queue).SyncRoot)
            {
                _queue.Enqueue(order);
            }
            ProduceCount++;
            Console.WriteLine(string.Format("Producer Thread:enqueue order of {0} user,create date: {1}", order.UserName, order.CreateDate));

            _syncEvents.NewItemEvent.Set();
        }

        private Queue<Order> _queue;

        private SyncEvents _syncEvents;

    }
}