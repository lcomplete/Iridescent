using System;

namespace QueuePrototype
{
    public class Order
    {
        public string UserName { get; set; }

        public string ScheduleId { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime HandleDate { get; set; }

    }
}