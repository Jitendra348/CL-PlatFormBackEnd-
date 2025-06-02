using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Models
{
    [Table("ExceptionLog")]
    public class ExceptionModel
    {
        [Key]
        public int ExceptionId { get; set; }

        [Required]
        public string ExceptionMessage { get; set; }

        [Required]
        public string StackTrace { get; set; }

        [Required]
        public string ControllerName { get; set; }

        [Required]
        public string ActionName { get; set; }

        [Required]
        public DateTime DateofException { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}
