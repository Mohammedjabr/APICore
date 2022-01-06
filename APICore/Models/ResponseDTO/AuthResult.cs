using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICore.Models.ResponseDTO
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string ErrorCode { get; set; }
    }
}
