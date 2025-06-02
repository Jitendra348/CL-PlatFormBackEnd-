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
    [Table("MissionMaster")]
    public class MissionModel
    {
        [Key]
        public int MissionId { get; set; }
        public string MissionTitle { get; set;}
        public string MissionDescription { get; set;}
        public string OrganizationName { get; set;}
        public string OrganizationDetail { get; set;}
        public DateTime StartDate { get; set;}
        public DateTime EndDate { get; set;}
        public int? TotalSeats { get; set;}
        public DateTime? RegistrationDeadline { get; set;}
        public int CountryId { get; set;}
        public int CityId { get; set;}
        public int ThemeId { get; set;}
        public string Skills { get; set;}
        public string Images { get; set;}
        public string Documents { get; set;}
        public string Availability { get; set;}
        [ValidateNever]
        public DateTime? CreatedDate { get; set;}
        [ValidateNever]
        public string? CreatedBy { get; set;}
        [ValidateNever]
        public DateTime? ModifiedDate { get; set;}
        [ValidateNever]
        public string? ModifiedBy { get; set;}
        public string MissionType { get; set;}
        public string? GoalObjective { get; set;}
        public string ShortDescription { get; set;}
        public int? Rate { get; set;}
    }
}
