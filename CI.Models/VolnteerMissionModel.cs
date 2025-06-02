using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Models
{
    public class VolnteerMissionModel
    {
        [Key]
        public int MissionId { get; set; }

        public string MissionTitle { get; set; }

        public string ShortDescription { get; set; }

        public string MissionDescription { get; set; }

        public string MissionType { get; set; }

        public string MissionTypeData { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Theme { get; set; }

        public string Skills { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationDetail { get; set; }

        public int? TotalSeats { get; set; }

        public DateTime? RegistrationDeadline { get; set; }  

        public string Availability { get; set; }

        public List<string> Images { get; set; }

        public List<string> Documnets { get; set; }

        public string? GoalObjective { get; set; }

        public int Rate { get; set; }
    }
}
