using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Models;
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

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
  
        private IAuthRepository repo;
        private IConfiguration config;
        //we are injectin config too cuz we need to use key/value properties in our App.json configuration file

        public AuthController(IAuthRepository repo,IConfiguration config)
        {
            this.repo = repo;
            this.config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> register(UserForRegistrationDTO userDto) //n background .net'i e bind( e krijon instancen edhe ja jep vlerat qe ja qojm n request)
        {
            userDto.Username = userDto.Username.ToLower();

            if (await repo.userExists(userDto.Username))
                return BadRequest("Username already exists");

            var userToCreate = new User
            {
                Username = userDto.Username
            };

            var createdUser = await repo.register(userToCreate, userDto.Password);

            return StatusCode(201);
        }
        [HttpPost("login")]
        public async Task<IActionResult> login (UserForLoginDTO userLoginDto)
        {
            //returns user if user exists and the passwords match
            var userFromRepo = await repo.logIn(userLoginDto.Username.ToLower(), userLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            //now we create a token so the user can use it to authentificate without going to dbs for every response
            //token will contain userID and userName

            //CREATATING our TOKEN
            var claims = new[]
            {
                //we are assiging userid and username of token
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.Username)
            };
            //we need a key to asign our token (its hashed not readable from our token)
            //we get the key from our Appsettings file we inject config in our controller so we can get the key
            //the key is important for the security of our API, with this key people can pretend to be a user without being one
            //we need a key for signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("AppSettings:Token").Value)); //gets super secret key 

            //this takes the key and the algorithem we use to hash this key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); //signingcredentials is part of the Token and used to check if valid user is requesting
            //the signature is differend, based on our key entered 

            //
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1), //24hours
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            //now we create the token and pass the tokenDescriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //now we return this to our client (we want to return it as object)

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            }); //we pass the token that we created here as object (this will be return at client as obj)

        }

    }
}
