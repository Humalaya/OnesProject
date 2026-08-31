using System;

namespace backend.Domain.Entities
{
    public class Schedule
    {
        public Guid ID { get; set; }
        public Guid ToDoID { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int Order { get; set; }
    }
}
