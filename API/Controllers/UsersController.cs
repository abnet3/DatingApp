using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // [ApiController]
    // [Route("api/[controller]")]
    // public class UsersController : ControllerBase

    // inherit from the baseapicontroller classswe created to code reuse

    [Authorize]
    public class UsersController : BaseApiController

    {
        // private readonly DataContext _context;
        // public UsersController(DataContext context)
        // {
        //     _context = context;

        // }
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;

        }





        [HttpGet]
        // [AllowAnonymous]
        // public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        public async Task<ActionResult<IEnumerable<MemberDtos>>> GetUsers()

        {
            //    return Ok(await _userRepository.GetUsersAsync()); 

            // var users = await _userRepository.GetUsersAsync();

            // var usersToReturn = _mapper.Map<IEnumerable <MemberDtos>>(users);
            // return Ok(usersToReturn);

             var users = await _userRepository.GetMemebersAsync();

             return Ok(users);


        }

        // [Authorize]
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDtos>> GetUser(string username)
  

        {
            // var user =  await _userRepository.GetUserByUsernameAsync(username);
                     return await _userRepository.GetMemeberAsync(username);




        }} 
}