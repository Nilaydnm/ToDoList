using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ToDoGroup : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? Deadline { get; set; }
        public List<ToDo> ToDos { get; set; }
    }
}
