using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using ZOV.Models;

namespace ZOV.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthNController : ControllerBase
    {
        private readonly AppDbContext Context;
        private readonly IConfiguration Configuration;

        public AuthNController(AppDbContext Context, IConfiguration Configuration)
        {
            this.Context = Context;
            this.Configuration = Configuration;
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] NewUser NewUser)
        {
            var BDUser = await Context.Users.SingleOrDefaultAsync(ZOV => ZOV.Login == NewUser.Login);

            if (BDUser != null)
            {
                if (BCrypt.Net.BCrypt.Verify(NewUser.Password, BDUser.Password))
                {
                    return Ok(new { Token = CreateJWToken(BDUser.Login!, Configuration["Jwt:Key"]!)});
                }
            }

            return Unauthorized();
        }

        [HttpPost("RegIn")]
        public async Task<IActionResult> RegIn([FromBody] NewUser NewUser)
        {
            if (NewUser.Login != null && NewUser.Password != null)
            {
                if (NewUser.Login.Count() < 3)
                {
                    return BadRequest("СЛИШКОМ КОРОТКИЙ ЛОГИН ИЛИ ПАРОЛЬ");
                }

                if (await Context.Users.AnyAsync(ZOV => ZOV.Login == NewUser.Login))
                {
                    return BadRequest("ДАННЫЙ ЛОГИН УЖЕ ЗАНЯТ");
                }

                if (!IllegalChar(NewUser.Login) || !IllegalChar(NewUser.Password))
                {
                    return BadRequest("НЕДОПУСТИМЫЕ СИМВОЛЫ");
                }

                string Salt = BCrypt.Net.BCrypt.GenerateSalt(10);

                var User = new User
                {
                    Login = NewUser.Login,
                    Password = BCrypt.Net.BCrypt.HashPassword(NewUser.Password, Salt),
                    Salt = Salt
                };

                Context.Users.Add(User);
                await Context.SaveChangesAsync();
                return Ok("УСПЕШНО");
            }

            return BadRequest("ЧТО-ТО СЛОМАЛОСЬ");
        }

        private bool IllegalChar(string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z0-9]+$");
        }

        private string CreateJWToken(string Login, string JWTKey)
        {
            var Claims = new[] { new Claim("Login", Login) };
            var TokenHandler = new JwtSecurityTokenHandler();
            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(Claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(JWTKey)), SecurityAlgorithms.HmacSha256Signature),
                Issuer = Configuration["Jwt:Issuer"],
                Audience = Configuration["Jwt:Audience"]
            };

            return TokenHandler.WriteToken(TokenHandler.CreateToken(TokenDescriptor));
        }
    }
}