using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


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
    }
}
