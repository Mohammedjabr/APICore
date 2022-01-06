
using APICore.Data;
using APICore.Models.Entities;
using APICore.Models.RequestDTO;
using APICore.Models.ResponseDTO;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace APICore.Repositories.Interfaces
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper map;
        private readonly IConfiguration configuration;

        public UserRepository(ApplicationDbContext context, IMapper map, IConfiguration configuration)
        {
            _context = context;
            this.map = map;
            this.configuration = configuration;
        }
        public AuthResult Registration(UserAddDTO userDTO)
        {
            if (_context.Users.Any(m => m.UserName == userDTO.UserName))
            {
                return new AuthResult { Success = false, ErrorCode = "Usr001" };
            }

            if (_context.Users.Any(m => m.Email == userDTO.Email))
            {
                return new AuthResult { Success = false, ErrorCode = "Usr002" };
            }

            var CurUser = map.Map<Users>(userDTO);
            byte[] hash, salt;
            GererateHash(userDTO.Password, out hash, out salt);
            CurUser.Passwordhash = hash;
            CurUser.Passwordsalt = salt;

            _context.Users.Add(CurUser);
            _context.SaveChanges();
            return new AuthResult { Success = true, UserId = CurUser.UserId, UserName = CurUser.UserName };
        }

        public AuthResult UserLogin(UserLoginDTO loginDTO)
        {
            var CurUser = _context.Users.Where(m => m.UserName.Equals(loginDTO.UserName)).SingleOrDefault();
            if (CurUser == null)
            {
                return new AuthResult { Success = false, ErrorCode = "Usr003" };
            }

            if (!ValidateHash(loginDTO.Password, CurUser.Passwordhash, CurUser.Passwordsalt))
            {
                return new AuthResult { Success = false, ErrorCode = "Usr004" };
            }
            //generate JWT Token
            var key = configuration.GetValue<string>("JWTSecret");
            var keyByte = Encoding.ASCII.GetBytes(key);
            var Desc = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyByte), SecurityAlgorithms.HmacSha512Signature),
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub,CurUser.UserName),
                         new Claim(JwtRegisteredClaimNames.Email,CurUser.Email),
                          new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                           new Claim("UserId",CurUser.UserId),
                    }
                    )
            };
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(Desc);
            return new AuthResult { Success = true, UserId = CurUser.UserId, UserName = CurUser.UserName,Token = handler.WriteToken(token) };
        }

        private bool ValidateHash(string password, byte[] passwordhash, byte[] passwordsalt)
        {
            using (var hash = new System.Security.Cryptography.HMACSHA512(passwordsalt))
            {
                var newPassHash = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < newPassHash.Length; i++)
                {
                    if (newPassHash[i] != passwordhash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void GererateHash(string Password, out byte[] PasswordHash, out byte[] PasswordSalt)
        {
            using (var hash = new System.Security.Cryptography.HMACSHA512())
            {
                PasswordHash = hash.ComputeHash(Encoding.UTF8.GetBytes(Password));
                PasswordSalt = hash.Key;
            }
        }
    }
}
