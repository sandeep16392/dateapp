using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DateApp.Core.Contracts.DAL;
using DateApp.Core.Contracts.Services;
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
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;

        public UsersController(IUsersService usersService, IMapper mapper)
        {
            _usersService = usersService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var users = await _usersService.GetUsers(userParams, currentUserId);

            return Ok(users);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var userDm = await _usersService.GetUser(id);
            return Ok(userDm);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDm userForUpdate)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            if (await _usersService.UpdateUser(id, userForUpdate))
                return NoContent();

            throw new Exception($"Updating user {id} failed on save.");
        }

        [HttpPost("{id}/like/{recepientId}")]
        public async Task<IActionResult> LikeUser(int id, int recepientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            try
            {
                if(await _usersService.LikeUser(id, recepientId))
                {
                    return Ok();
                }
            }
            catch (ArgumentException ex)
            {

                return BadRequest(ex.Message);
            }
            catch(NotSupportedException)
            {
                return NotFound();
            }

            return BadRequest("Failed to Like user.");
        }
    }
}