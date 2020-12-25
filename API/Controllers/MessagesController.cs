using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository,
                                  IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }


        [HttpPost]

        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {

            var username = User.GetUserName();

            if (username == createMessageDto.recipientsUsername?.ToLower())
                return BadRequest("You can not send message to your self");

            var sender = await _userRepository.GetUserByUsernameAsync(username);

            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto?.recipientsUsername);

            if (recipient == null) return NotFound();

            var message = new Message
            {

                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDto.content
            };

            _messageRepository.AddMessage(message);

            if(await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send the message");




        }
        
    [HttpGet]

    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesFor([FromQuery] 
    MessageParams messageParams){

        messageParams.Username = User.GetUserName();


        var messages = await _messageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

        return messages;
    }


    [HttpGet("thread/{username}")]

    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {

          var currentUsername = User.GetUserName();

          return Ok(await _messageRepository.GetMessageThread(currentUsername, username));

    }



             [HttpDelete("{id}")]
             public async Task<ActionResult> DeleteMessaage(int id){

                 var username = User.GetUserName();

                 var message = await _messageRepository.GetMessage(id);

                 if(message.Sender.UserName != username && message.Recipient.UserName != username)
                 return Unauthorized();

                 if(message.Sender.UserName == username) message.SenderDeleted = true;

                 if(message.Recipient.UserName == username) message.RecipientDeleted = true;

                 if(message.SenderDeleted && message.RecipientDeleted) _messageRepository.DeleteMessage(message);

                 if(await _messageRepository.SaveAllAsync()) return Ok();

                 return BadRequest("Problem deleting the message");


             }



    }


    
}