using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Models
{
    public class MissionVolunteeringResponse
    {
        [Key]
        public int VolunteerId { get; set; }

        public string MissionTitle { get; set; }

        public int MissionId { get; set; }

        public int UserId { get; set; }

        public string? UserName { get; set; }

        public DateTime? AppliedDate { get; set; }
    }
}
