using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Models
{
    public class VolunteerTimeResponse
    {
        [Key]
        public int VolunteerId { get; set; }

        public string Mission { get; set; }

        public DateTime Date { get; set; }

        public string Hours { get; set; }

        public string Minutes { get; set; }

        public string? Message { get; set; }
    }
}
