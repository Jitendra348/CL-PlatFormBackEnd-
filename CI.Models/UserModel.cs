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
    [Table("UserMaster")]
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string? EmployeeId { get; set; }

        public string? Manager { get; set; }

        public string? Title { get; set; }

        public int? DepartmentId { get; set; }

        public string? ProfileSummary { get; set; }

        public string? VolunteerReasons { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required]
        public int CountryId { get; set; }

        public string? Availability { get; set; }

        public string? Skills { get; set; }

        public bool Status { get; set; }

        public string? ProfilePic { get; set; }

        [ValidateNever]
        public DateTime? CreatedDate { get; set;}

        [ValidateNever]
        public string? CreatedBy { get; set; }

        [ValidateNever]
        public DateTime? ModifiedDate { get; set; }

        [ValidateNever]
        public string? ModifiedBy { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
