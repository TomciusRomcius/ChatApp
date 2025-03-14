using System.Collections;
using System.Security.Claims;
using ChatApp.Domain.Entities.UserFriend;
using ChatApp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Server.Presentation.UserFriend
{
    [ApiController]
    [Route("[controller]")]
    public class UserFriendController : ControllerBase
    {
        readonly DatabaseContext _databaseContext;

        public UserFriendController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        [HttpGet]
        public IActionResult GetUserFriends([FromBody] AddFriendDto dto)
        {
            // Where the given user is the initiator and a friend is a receiver

            var query1 = from friend in _databaseContext.UserFriends
                         join receiver in _databaseContext.Users
                         on friend.ReceiverId equals receiver.Id
                         where friend.InitiatorId == dto.UserId
                         select new
                         {
                             UserId = receiver.Id,
                             UserName = receiver.UserName
                         };

            // Where the given user is the receiver and a friend is an initiator

            var query2 = from friend in _databaseContext.UserFriends
                         join initiator in _databaseContext.Users
                         on friend.InitiatorId equals initiator.Id
                         where friend.ReceiverId == dto.UserId
                         select new
                         {
                             UserId = initiator.Id,
                             UserName = initiator.UserName
                         };

            ArrayList result = new ArrayList();

            foreach (var row in query1)
            {
                result.Add(row);
            }


            foreach (var row in query2)
            {
                result.Add(row);
            }

            return Ok(result);
        }

        [HttpPost("request")]
        public async Task<IActionResult> SendFriendRequest([FromBody] AddFriendDto addFriendDto)
        {
            // TODO: check if already friends

            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            if (addFriendDto.UserId == userId)
            {
                return BadRequest("You cannot add yourself :(");
            }

            await _databaseContext.UserFriends.AddAsync(
                new UserFriendEntity
                {
                    InitiatorId = userId,
                    ReceiverId = addFriendDto.UserId,
                    Status = UserFriendEntity.StatusToByte(UserFriendStatus.REQUEST),
                }
            );

            await _databaseContext.SaveChangesAsync();

            return Created();
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptFriendRequest([FromBody] AddFriendDto dto)
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            UserFriendEntity? instance = _databaseContext.UserFriends.FirstOrDefault((item) => item.InitiatorId == dto.UserId && item.ReceiverId == userId);

            if (instance is not null)
            {
                instance.Status = UserFriendEntity.StatusToByte(UserFriendStatus.FRIEND);
                await _databaseContext.SaveChangesAsync();
            }

            else
            {
                return BadRequest("The user is not inviting you to the friends");
                // handle
            }

            return Ok();
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromFriends([FromBody] AddFriendDto dto)
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            if (userId == dto.UserId)
            {
                return BadRequest("Trying to remove yourself from friends");
            }

            await _databaseContext.UserFriends.Where((uf) => uf.InitiatorId == userId && uf.ReceiverId == dto.UserId).ExecuteDeleteAsync();
            await _databaseContext.UserFriends.Where((uf) => uf.ReceiverId == userId && uf.InitiatorId == dto.UserId).ExecuteDeleteAsync();

            return Created();
        }
    }
}
