using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Models
{
    public class MissionSkillResponse
    {
        [Key]
        public int SkillId { get; set; }

        public string SkillTitle { get; set; }

        public string Status { get; set; }
    }

    public class MissionSkillVM
    {
        public List<MissionSkillResponse> MissionSkills { get; set; }

        public int TotalCount { get; set; }
    }
}
