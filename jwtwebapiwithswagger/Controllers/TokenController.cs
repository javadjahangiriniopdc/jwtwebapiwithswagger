using jwtwebapiwithswagger.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace jwtwebapiwithswagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        public IConfiguration _configuration;
        public readonly ApplicationDBContext _context;
        public TokenController(IConfiguration configuration, ApplicationDBContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> post(Userinfo userinfo)
        {
            if (userinfo != null && userinfo.UserName != null && userinfo.Password != null)
            {
                var user = await GetUser(userinfo.UserName, userinfo.Password);
                if (user != null)
                {
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id",user.UserId.ToString()),
                        new Claim("UserName",user.UserName),
                        new Claim("Password",user.Password)
                    };

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                       _configuration["Jwt:Audience"],
                       claims,
                       expires: DateTime.Now.AddMinutes(20),
                       signingCredentials: credentials);
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid Cerdentials");
                }

               
            }
            else
            {
                return BadRequest("Invalid Cerdentials");
            }
        }
        [HttpGet]
        private async Task<Userinfo> GetUser(string username,string pass)
        {
            return await _context.Userinfo.FirstOrDefaultAsync(u => u.UserName == username && u.Password == pass);
        }

    }
}
