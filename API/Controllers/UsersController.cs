using System.Security.Claims;
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
using Microsoft.AspNetCore.Http;
using API.Extensions;
using API.Helpers;

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
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;

        }





        // [HttpGet]
        // [AllowAnonymous]
        // public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        //
            ////    return Ok(await _userRepository.GetUsersAsync()); 

            //// var users = await _userRepository.GetUsersAsync();

            //// var usersToReturn = _mapper.Map<IEnumerable <MemberDtos>>(users);
            //// return Ok(usersToReturn);

        //     var users = await _userRepository.GetMemebersAsync();

        //     return Ok(users);


        // }

                  [HttpGet]

        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers([FromQuery]UserParams userParams)
        {
        
            //    return Ok(await _userRepository.GetUsersAsync()); 

            // var users = await _userRepository.GetUsersAsync();

            // var usersToReturn = _mapper.Map<IEnumerable <MemberDtos>>(users);
            // return Ok(usersToReturn);
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            userParams.CurrentUsername = user.UserName;

            if(string.IsNullOrEmpty(userParams.Gender))
            
            userParams.Gender = user.Gender == "male"? "female" : "male";

            var users = await _userRepository.GetMemebersAsync(userParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, 
            users.TotalCount, users.TotalPages);

            return Ok(users);


        }

        // [Authorize]
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDtos>> GetUser(string username)


        {
            // var user =  await _userRepository.GetUserByUsernameAsync(username);
            return await _userRepository.GetMemeberAsync(username);




        }

        [HttpPut]

        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {

            // var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //gets the username form the token to be updated // i added an extension to reuse this ClaimPrincpalextension
            var username = User.GetUserName();
            var user = await _userRepository.GetUserByUsernameAsync(username);


            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed Update User");

        }

        [HttpPost("add-photo")]

        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {


            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            var result = await _photoService.AddPhotoAsync(file);

            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo {

                Url = result.SecureUrl.AbsoluteUri,
                publicId = result.PublicId,

            };

            if(user.Photos.Count == 0){

                photo.IsMain = true;
            }
       
                  user.Photos.Add(photo);

                  if(await _userRepository.SaveAllAsync()) 
                //   return _mapper.Map<PhotoDto>(photo);
                  return CreatedAtRoute("GetUser", new {username = user.UserName}, _mapper.Map<PhotoDto>(photo));

                 return BadRequest("Problem Adding Photo");
                }


                [HttpPut("set-main-photo/{photoID}")]

                public async Task<ActionResult> SetMainPhoto(int photoId){

                      var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

                      var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

                      if(photo.IsMain) return BadRequest("This is already your main photo");

                      var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

                      if(currentMain != null) currentMain.IsMain = false;

                      photo.IsMain = true;

                      if(await _userRepository.SaveAllAsync()) return NoContent();

                      return BadRequest("Failed to set main photo");
                }


            [HttpDelete("delete-photo/{photoId}")]
            public async Task<ActionResult> DeletePhoto(int photoId){

                var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

                var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

                if(photo == null)return NotFound();

                if(photo.IsMain) return BadRequest("You cannot delete your main photo");

                if(photo.publicId != null){

                  var result =  await _photoService.DeletePhotoAsync(photo.publicId);
                    if(result.Error != null) return BadRequest(result.Error.Message);
                }

                user.Photos.Remove(photo); 

                if (await _userRepository.SaveAllAsync()) return Ok();

                return BadRequest("Failed to delte the user");
            }

    }


}