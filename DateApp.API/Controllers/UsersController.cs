using System;
using System.Collections.Generic;
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
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var currentUser = await _repository.GetUser(currentUserId);
            userParams.UserId = currentUserId;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
            }


            var users = await _repository.GetUsers(userParams);

            var userDms = _mapper.Map<IEnumerable<UserListDm>>(users);
            var paginatedResp = new PaginateResponseDm
            {
                Pagination = new PaginationDm(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages),
                Users = userDms
            };
            //Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(paginatedResp);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repository.GetUser(id);
            var userDm = _mapper.Map<UserDetailsDm>(user);
            return Ok(userDm);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDm userForUpdate)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repository.GetUser(id);
            var userToSave = _mapper.Map(userForUpdate, userFromRepo);

            if (await _repository.SaveAll())
                return NoContent();

            throw new Exception($"Updating user {id} failed on save.");
        }

        [HttpPost("{id}/like/{recepientId}")]
        public async Task<IActionResult> LikeUser(int id, int recepientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var like = await _repository.GetLike(id, recepientId);

            if (like != null)
                return BadRequest("You already Like this User");

            if (await _repository.GetUser(recepientId) == null)
                return NotFound();

            like = new Like
            {
                LikerId = id,
                LikeeId = recepientId
            };

            _repository.Add(like);

            if (await _repository.SaveAll())
                return Ok();

            return BadRequest("Failed to Like user.");
        }
    }
}