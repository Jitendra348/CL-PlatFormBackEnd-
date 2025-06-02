using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Models
{
    [Table("City")]
    public class CityModel
    {
        [Key]
        public int CityId { get; set; }

        public string CityName { get; set; }

        public int CountryId { get; set; }
    }
}
