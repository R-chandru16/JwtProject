using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

//Added...

using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;
using System.Text;

using DemoProjectJwtToken.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace TraineeService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TokenController : ControllerBase
    {
        public readonly IConfiguration _configuration;
        private readonly JwtDemoDbContext _context;
        public TokenController(IConfiguration config, JwtDemoDbContext context)
        {
            _configuration = config;
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Post(Trainee _trainee)
        {
            if (_trainee != null && _trainee.Email != null && _trainee.Password != null)
            {
                var user = await GetTrainee(_trainee.Email, _trainee.Password);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Email", user.Email),
                    new Claim("Password", user.Password)
                   };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task<Trainee> GetTrainee(string email, string password)
        {
            return await _context.Trainees.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }
    }
}