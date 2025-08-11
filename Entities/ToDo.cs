using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ToDo : BaseEntity
    {
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int? GroupId { get; set; }
        public ToDoGroup Group { get; set; }
        public DateTime? Deadline { get; set; }
        public bool IsDeleted { get; set; } = false;


    }
}
