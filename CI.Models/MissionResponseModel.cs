using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Models
{
    public class MissionResponseModel
    {
        [Key]
        public int MissionId { get; set; }

        public string MissionTitle { get; set; }

        public string MissionType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class ViewMissionResponse
    {
        public List<MissionResponseModel> Missions { get; set; }

        public int totalRecords { get; set; }
    }
}
