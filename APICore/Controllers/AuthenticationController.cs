using APICore.helper;
using APICore.Models.RequestDTO;
using APICore.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRepository userRep;
        private readonly IErrorClass error;

        public AuthenticationController(IUserRepository UserRep, IErrorClass error)
        {
            userRep = UserRep;
            this.error = error;
        }

        [HttpPost("Registration")]
        public IActionResult Registration(UserAddDTO UserDTO)
        {
            var AuthResult = userRep.Registration(UserDTO);
            if (AuthResult.Success)
            {
                return Ok(AuthResult);
            }
            error.LoadError(AuthResult.ErrorCode);
            ModelState.AddModelError("UserName", error.ErrorMessage);
            return ValidationProblem();
        }

        [HttpPost("Login")]
        public IActionResult Login(UserLoginDTO UserLoginDTO)
        {
            var AuthResult = userRep.UserLogin(UserLoginDTO);
            if (AuthResult.Success)
            {
                return Ok(AuthResult);
            }
            error.LoadError(AuthResult.ErrorCode);
            ModelState.AddModelError(error.ErrorProp, error.ErrorMessage);
            return ValidationProblem();
        }
    }
}
