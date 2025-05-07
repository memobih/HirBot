using HirBot.Data.Enums;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jop.Services.DataTransferObjects
{
    public class AddEmployeeDto 

    {

        [Required(ErrorMessage = "This field is Required")]
        [DataType(DataType.EmailAddress  ,ErrorMessage ="is not email")]
        public string email { get; set; }

        [Required(ErrorMessage = "This field is Required")]
        public string title { get; set; }
        [Required(ErrorMessage = "This field is Required")]
        public EmployeeType jobType { get; set; }
        [Required(ErrorMessage = "This field is Required")]
        public LocationType workType { get; set; }
        [Required(ErrorMessage = "This field is Required")]
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        [Required(ErrorMessage = "This field is Required")]

        public int  rate { get; set; }
        [Required(ErrorMessage = "This field is Required")]

        public string? location { get; set; }
    }
}
