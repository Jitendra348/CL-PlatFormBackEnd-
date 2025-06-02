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
    [Table("MissionRating")]
    public class MissionRatingModel
    {
        [Key]
        public int RateId { get; set; }

        [Required]
        public int MissionId { get; set; }

        [Required]
        public int Rate { get; set; }

        [ValidateNever]
        public string? CreatedBy { get; set; }

        [ValidateNever]
        public DateTime? CreatedDate { get; set; }
    }
}
