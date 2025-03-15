using System.Security.Claims;
using ChatApp.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Server.Presentation.UserFriend
{
    [ApiController]
    [Route("[controller]")]
    public class UserFriendController : ControllerBase
    {
        readonly IUserFriendService _userFriendService;

        public UserFriendController(IUserFriendService userFriendService)
        {
            _userFriendService = userFriendService;
        }

        [HttpGet]
        public IActionResult GetUserFriends([FromBody] AddFriendDto dto)
        {
            var result = _userFriendService.GetUserFriends(dto.UserId);
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

            await _userFriendService.SendFriendRequest(userId, addFriendDto.UserId);

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

            await _userFriendService.AcceptFriendRequest(dto.UserId, userId);

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

            await _userFriendService.RemoveFromFriends(userId, dto.UserId);

            return Created();
        }
    }
}
