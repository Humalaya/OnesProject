using System;

namespace backend.Domain.Entities
{
    public class ToDo
    {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public int Priority { get; set; } = 1; // 0 = Low, 1 = Medium, 2 = High
        public string? Tags { get; set; } // comma-separated
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
