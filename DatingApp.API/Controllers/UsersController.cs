using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;


namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository repository;
        private readonly IMapper mapper;

        public UsersController(IDatingRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> getUsers()
        {
            var users = await repository.getUsers();

            var usersToReturn = mapper.Map<IEnumerable<UserForListDTO>>(users);

            return Ok(usersToReturn);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> getUser(int id)
        {
            var user = await repository.getUser(id);
            
            //Map method excecutes mapping from source object to destination object
            //UserForDetailedDTO is the destenation and the source is user that we passed in parameter
            var userToReturn = mapper.Map<UserForDetailedDTO>(user);

            //now we return user filtered returns only atributes specified in DTO
            return Ok(userToReturn);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> updateUser(int id,UserForUpdateDTO userForUpdateDto)
        {
            //check if id of the path/route is match with the one that user has in his token
            //if they dont match it means that client is not authorized to update the user
            //we check if the user thats trying to update is the same user from token
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }
            var userFromRepo = await repository.getUser(id);
            //Now we need to take the information that comes to our DTO and map it with the userFromRepo
            //this method will take values from userForUpdateDto and write it /store it in userFromRepo
            mapper.Map(userForUpdateDto,userFromRepo);

            if(await repository.saveAll()){
                
                return NoContent();
            }
            throw new Exception($"Updating user {id} failed to save");
        }
    }
}
