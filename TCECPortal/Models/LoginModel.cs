using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCECPortal.Models
{
    public class LoginModel
    {
        public string Email { get; set; }
        public string userCode { get; set; }
        public string Pssword { get; set; } 
    }
}
