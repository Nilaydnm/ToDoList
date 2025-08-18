using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs
{
    public class ToDoGroupStatsDto
    {
        public int GroupId { get; set; }
        public string Title { get; set; } = string.Empty;

        public int Total { get; set; }
        public int Completed { get; set; }
        public int Active { get; set; }
        public int Overdue { get; set; }
    }
}
