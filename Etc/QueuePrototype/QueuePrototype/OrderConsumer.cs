using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace QueuePrototype
{
    public class OrderConsumer
    {
        public int ConsumeCount { get; private set; }

        public OrderConsumer(Queue<Order> q, SyncEvents e)
        {

            _queue = q;

            _syncEvents = e;

        }

        public void ThreadRun()
        {
            int waitResult;
            while ( (waitResult= WaitHandle.WaitAny(_syncEvents.EventArray)) != 1)//1 为退出信号
            {
                if (waitResult == 0)//0为队列新增订单信号
                {
                    Order item;
                    lock (((ICollection) _queue).SyncRoot)
                    {
                        while (_queue.Count>0)
                        {
                            item = _queue.Dequeue();

                            //handle
                            item.HandleDate = DateTime.Now;
                            Console.WriteLine("Consumer Thread:consuming order of {0} user", item.UserName);
                            Thread.Sleep(200);
                            Console.WriteLine("Consumer Thread:consumed order of {0} user, handle date: {1}", item.UserName,
                                              item.HandleDate);

                            ConsumeCount++;
                        }
                    }
                }
            }

            Console.WriteLine("Consumer Thread: consumed {0} items", ConsumeCount);
        }

        private Queue<Order> _queue;

        private SyncEvents _syncEvents;

    }
}