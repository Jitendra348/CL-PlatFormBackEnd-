using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Models
{
    public class MissionThemeResponse
    {
        [Key]
        public int ThemeId { get; set; }

        public string ThemeName { get; set; }

        public string Status { get; set; }
    }

    public class MissionThemeVM
    {
        public List<MissionThemeResponse> MissionThemes { get; set; }   

        public int TotalCount { get; set; }
    }
}
