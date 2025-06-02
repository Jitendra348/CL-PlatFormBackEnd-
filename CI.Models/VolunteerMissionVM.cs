using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Models
{
    public class VolunteerMissionVM
    {
        public List<VolnteerMissionModel> volnteerMissions { get; set; }

        public List<string> country { get; set; }

        public List<string> city { get; set; }

        public List<string> themes { get; set; }

        public List<string> skills { get; set; }

        public int totalCount { get; set; }

        public List<int> favMissions { get; set; }
    }
}
