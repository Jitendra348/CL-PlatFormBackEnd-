using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Models
{
    public class MissionVM
    {
        public MissionRequestModel Mission { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CityList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CountryList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> ThemeList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> SkillList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> AvailabilityList { get; set; }
    }
}
