using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Models
{
    [Table("MissionSkills")]
    public class MissionSkillModel
    {
        [Key]
        public int SkillId { get; set; }

        [Required]
        public string SkillTitle { get; set; }

        [Required]
        public bool Status { get; set; }

        [ValidateNever]
        public DateTime? CreatedDate { get; set; }

        [ValidateNever]
        public string? CreatedBy { get; set; }

        [ValidateNever]
        public DateTime? ModifiedDate { get; set; }

        [ValidateNever]
        public string? ModifiedBy { get; set; }
    }
}
