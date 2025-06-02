using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Models
{
    public class ForgetPasswordModel
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [Url]
        public string ClientURI { get; set; }
    }
}
