using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Models
{
    public class VolunteerGoalResponse
    {
        [Key]
        public int Id { get; set; }

        public string Mission { get; set; }

        public string Actions { get; set; }

        public DateTime Date { get; set; }

        public string? Message { get; set; }
    }
}
