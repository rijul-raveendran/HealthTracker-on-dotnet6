using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Authentication.Models.DTO.Outgoing
{
    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; }
        public List<string> Errors { get; set; }
    }
}
