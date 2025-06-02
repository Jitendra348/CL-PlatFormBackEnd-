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
    [Table("FavoriteMissions")]
    public class FavMissionModel
    {
        [Key]
        public int FavMissionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string MissionId { get; set; }

        [ValidateNever]
        public string? CreatedBy { get; set; }

        [ValidateNever]
        public DateTime? CreatedDate { get; set; }

        [ValidateNever]
        public string? ModifiedBy { get; set; }

        [ValidateNever]
        public DateTime? ModifiedDate { get; set; }
    }
}
