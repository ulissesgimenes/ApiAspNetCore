using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PrimeiraApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PrimeiraApi.Controllers
{
    [ApiController]
    [Route("api/conta")]
    public class AuthController : ControllerBase
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;

        public AuthController(SignInManager<IdentityUser> signInManager,
                                    UserManager<IdentityUser> userManager,
                                    IOptions<JwtSettings> jwtSettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
        }


        [HttpPost("Registrar")]        
        public async Task<ActionResult> Registrar(UserRegisterViewModel UserRegistrerV)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            } 

            var user = new IdentityUser
            {
                UserName = UserRegistrerV.Email,
                Email = UserRegistrerV.Email,
                EmailConfirmed = true                
            };

            var result = await _userManager.CreateAsync(user, UserRegistrerV.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return Ok(await GerarJwt(UserRegistrerV.Email));
            }

            return Problem("Falha ao registar usuário.");
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginViewModel UserLoginV)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(UserLoginV.Email, UserLoginV.Password, false, true);

            if (result.Succeeded)
            {                
                return Ok(await GerarJwt(UserLoginV.Email));
            }

            return Problem("Usuário ou senha incorretos.");
        }

        private async Task<string> GerarJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.keySecurity);

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtSettings.Sender,
                Audience = _jwtSettings.Audience,
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token);

            return encodedToken;
        }


    }
}
