using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCECPortal.Models
{
    public class AttendanceModel
    { 
        public int UserId { get; set; }
        public DateTime? AttendanceDate { get; set; } 
        public string DeviceName { get; set; }
        public string CreatedBy { get; set; } 
        public string GeneratedCode { get; set; }
    }
}
