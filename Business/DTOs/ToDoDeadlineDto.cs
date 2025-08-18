using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs
{
    public class ToDoDeadlineDto
    {
        public int ToDoId { get; set; }
        public DateTime? Deadline { get; set; }

        public TimeSpan? Delta { get; set; }

        public bool IsOverdue => Deadline.HasValue && Delta.HasValue && Delta.Value < TimeSpan.Zero;

        public string Humanized
        {
            get
            {
                if (Deadline is null) return "Son tarih yok";
                var d = Delta ?? TimeSpan.Zero;
                var abs = d.Duration();

                var parts = new List<string>();
                if (abs.Days > 0) parts.Add($"{abs.Days} gün");
                if (abs.Hours > 0) parts.Add($"{abs.Hours} saat");
                if (abs.Minutes > 0) parts.Add($"{abs.Minutes} dk");
                if (parts.Count == 0) parts.Add("0 dk");

                var text = string.Join(" ", parts);

                if (d < TimeSpan.Zero) return $"{text} gecikti";
                return $"{text} kaldı";
            }
        }
    }
}
