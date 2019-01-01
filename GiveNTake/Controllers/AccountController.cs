using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GiveNTake.Model;
using GiveNTake.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GiveNTake.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationUserDTO registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(registration.Email);

            //if (registration.Email.Contains("admin"))
            //{
            //    await _userManager.AddToRoleAsync(user, "Admin");
            //}


            if (user != null)
            {
                SerializableError serializableError = new SerializableError { { nameof(registration.Email), "Email already exist in the system" } };
                return this.StatusCode((int)HttpStatusCode.Conflict, serializableError);
            }

            User newUser = new User
            {
                Email = registration.Email,
                UserName = registration.Email,
                Id = registration.Email
            };

            var result = await _userManager.CreateAsync(newUser, registration.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            user = await _userManager.FindByEmailAsync(registration.Email);
            await _userManager.AddClaimAsync(user, new Claim("registration-date", DateTime.UtcNow.ToString("yy-MM-dd")));
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<SuccessfulLoginResult>> Login([FromBody] LoginUserDTO login)
        {
            SignInResult result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);
            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            User user = await _userManager.FindByEmailAsync(login.Email);
            JwtSecurityToken token = await GenerateTokenAsync(user);  //defined
            string serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new SuccessfulLoginResult() { Token = serializedToken });
        }

        private async Task<JwtSecurityToken> GenerateTokenAsync(User user)
        {
            var claims = new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var expirationDays = _configuration.GetValue<int>("JWTConfiguration:TokenExpirationDays");
            var signingKey = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWTConfiguration:SigningKey"));
            var token = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("JWTConfiguration:Issuer"),
                audience: _configuration.GetValue<string>("JWTConfiguration:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(expirationDays)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        [Authorize]
        [HttpGet("Email")]
        public ActionResult<string> GetEmail()
        {
            return Ok(User.Identity.Name);
        }
    }
}
