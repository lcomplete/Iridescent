using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace QueuePrototype
{
    class Program
    {
        private static OrderProducer orderProducer;
        private static OrderConsumer orderConsumer;

        private static void ShowQueueContents(Queue<Order> q)
        {
            Console.Write("current queque: ");
            lock (((ICollection)q).SyncRoot)
            {
                foreach (Order order in q)
                {
                    Console.Write("{0} ", order.UserName.ToString());
                }
            }

            Console.WriteLine();
            Console.WriteLine("current produce count: {0}, current consume count: {1}", orderProducer.ProduceCount, orderConsumer.ConsumeCount);
        }


        static void Main(string[] args)
        {
            SyncEvents syncEvents = new SyncEvents();
            Queue<Order> queue = new Queue<Order>();

            Console.WriteLine("Configuring worker threads...");

            orderProducer = new OrderProducer(queue, syncEvents);
            orderConsumer = new OrderConsumer(queue, syncEvents);

            Thread consumerThread = new Thread(orderConsumer.ThreadRun);
            Console.WriteLine("Launching consumer threads...");
            consumerThread.Start();

            Console.WriteLine("USAGE: \n\tusername: enqueue order \n\t[username],[username]: enqueue multi order \n\t/show: display current queue \n\t/stop: close queue service \n\t/quit: exit this program");

            string input;
            while (!string.Equals((input = Console.ReadLine()), "/quit", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("please input command");
                }
                else
                {
                    bool isBreak=false;
                    switch (input.ToLower())
                    {
                        case "/stop":
                            Console.WriteLine("Signaling threads to terminate...");
                            syncEvents.ExitThreadEvent.Set();
                            Console.WriteLine("main thread waiting for threads to finish...");
                            consumerThread.Join();
                            Console.WriteLine("service stoped");
                            isBreak = true;
                            break;
                        case "/show":
                            ShowQueueContents(queue);
                            break;
                        default:
                            foreach (var username in input.Split(','))
                            {
                                orderProducer.EnqueueOrder(new Order() { CreateDate = DateTime.Now, UserName = username });
                            }
                            break;
                    }
                    if (isBreak)
                    {
                        Console.ReadKey();
                        break;
                    }
                }
            }

            syncEvents.ExitThreadEvent.Set();
        }
    }
}
