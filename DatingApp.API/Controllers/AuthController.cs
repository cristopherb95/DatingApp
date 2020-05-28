using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DatingApp.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [AllowAnonymous]
  public class AuthController : ControllerBase
  {
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    public AuthController(IConfiguration config, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
    {
      _signInManager = signInManager;
      _userManager = userManager;
      _config = config;
      _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
    {
      var userToCreate = _mapper.Map<User>(userForRegisterDto);

      var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

      var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);

      if (result.Succeeded)
      {
        return CreatedAtRoute("GetUser", new { controller = "Users", id = userToCreate.Id }, userToReturn);
      }

      return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
    {
      var user = await _userManager.FindByNameAsync(userForLoginDto.Username);

      var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

      if (result.Succeeded)
      {
        var userToReturn = _mapper.Map<UserForListDto>(user);
        return Ok(new
        {
          token = generateJwtToken(user),
          user = userToReturn
        });
      }

      return Unauthorized();
    }

    private async Task<string> generateJwtToken(User user)
    {
      var claims = new List<Claim>
     {
       new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
       new Claim(ClaimTypes.Name, user.UserName)
     };

      var userRoles = await _userManager.GetRolesAsync(user);

      foreach(var role in userRoles) 
      {
        claims.Add(new Claim(ClaimTypes.Role, role));
      }

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.Now.AddDays(1),
        SigningCredentials = creds
      };

      var tokenHandler = new JwtSecurityTokenHandler();

      var token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token);
    }

  }
}