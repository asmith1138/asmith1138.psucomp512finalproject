using EHR.Data.Models;
using EHR.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EHR.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _config;


        public TokenController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateToken([FromBody] UserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                var role = await _roleManager.FindByIdAsync(user.RoleId.ToString());

                if (user != null)
                {
                    
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (result.Succeeded)
                    {

                        var claims = new[]
                        {
                          new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                          new Claim(ClaimTypes.Name, user.UserName),
                          new Claim(ClaimTypes.Email, user.Email),
                          //new Claim(ClaimTypes.NameIdentifier, user.Id),
                          new Claim(ClaimTypes.Role,user.Role.Name)
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                          _config["Tokens:Issuer"],
                          claims,
                          notBefore: DateTime.Now,
                          expires: DateTime.Now.AddDays(2),
                          signingCredentials: creds);

                        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
                    }
                }
            }

            return BadRequest("Could not create token");
        }

        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password == model.ConfirmPassword)
                {
                    var result = await _userManager.CreateAsync(new Data.Models.User { UserName = model.Username, Email = model.Email, RoleId = "0" }, model.Password);

                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(result.Errors);
                    }
                }
                return BadRequest("Passwords do not match");
            }
            return BadRequest("Could not create token");
        }
    }
}
