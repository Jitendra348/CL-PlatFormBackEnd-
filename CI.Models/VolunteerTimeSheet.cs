using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Models
{
    public class VolunteerTimeSheet
    {
        public List<VolunteerTimeResponse> volunteerTime { get; set; }
        public List<VolunteerGoalResponse> volunteerGoal { get; set; }
    }
}
