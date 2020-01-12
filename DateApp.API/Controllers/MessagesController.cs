using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DateApp.Core.Contracts.DAL;
using DateApp.Core.DataModels;
using DateApp.Core.EntityModels;
using DateApp.Core.Helpers;
using DatingApp.API.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivityFilter))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessagesController(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _messageRepository.GetMessage(id);

            if (messageFromRepo == null)
                return NotFound();

            return Ok(messageFromRepo);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams msgParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            msgParams.UserId = userId;

            var msgsFromRepo = await _messageRepository.GetMessagesForUser(msgParams);

            var msgs = _mapper.Map<IEnumerable<MessageToReturnDm>>(msgsFromRepo);

            var paginatedMsgs = new PaginatedMessageDm
            {
                Pagination = new PaginationDm(msgsFromRepo.CurrentPage, msgsFromRepo.PageSize, msgsFromRepo.TotalCount,
                    msgsFromRepo.TotalPages),
                Messages = msgs
            };

            return Ok(paginatedMsgs);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessagesThread(int recipientId, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messages = await _messageRepository.GetMessageThread(userId, recipientId);

            var msgThread = _mapper.Map<IEnumerable<MessageToReturnDm>>(messages);

            return Ok(msgThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDm messageForCreation)
        {
            var sender = await _messageRepository.GetUser(userId);
            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreation.SenderId = userId;

            var recipient = _messageRepository.GetUser(messageForCreation.RecipientId);

            if (recipient == null)
                return NotFound("Could not find User!");

            var message = _mapper.Map<Message>(messageForCreation);

            _messageRepository.Add(message);

            if (await _messageRepository.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageToReturnDm>(message);
                return Ok(messageToReturn);
            }

            throw new Exception("Creating new message failed on Save.");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var msgFromRepo = await _messageRepository.GetMessage(id);

            if (msgFromRepo.SenderId == userId)
                msgFromRepo.SenderDeleted = true;

            if (msgFromRepo.RecipientId == userId)
                msgFromRepo.RecipientDeleted = true;

            if (msgFromRepo.SenderDeleted && msgFromRepo.RecipientDeleted)
                _messageRepository.Delete(msgFromRepo);

            if (await _messageRepository.SaveAll())
                return NoContent();

            throw new Exception("Error deleting the message.");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageRead(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var msgFromRepo = await _messageRepository.GetMessage(id);

            if (msgFromRepo.RecipientId != userId)
                return Unauthorized();

            msgFromRepo.IsRead = true;
            msgFromRepo.DateRead = DateTime.Now;

            await _messageRepository.SaveAll();

            return NoContent();
        }
    }
}