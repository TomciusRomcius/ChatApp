using System.Collections;
using System.Security.Claims;
using ChatApp.Domain.Utils;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ChatApp.Domain.Entities.UserFriend;
using ChatApp.Presentation.Utils;

namespace ChatApp.Presentation.UserFriend
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
        public IActionResult GetUserFriends([FromQuery] GetFriendsDto dto)
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            Result<ArrayList> result = _userFriendService.GetUserFriends(dto.UserId ?? userId, dto.Status ?? UserFriendStatus.FRIEND);

            if (!result.IsError()) return Ok(result.GetValue());
            return ControllerUtils.OutputErrorResult(result.Errors.First());
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

            ResultError? error = await _userFriendService.SendFriendRequest(userId, addFriendDto.UserId);
            if (error is null) return Created();
            return ControllerUtils.OutputErrorResult(error);
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptFriendRequest([FromBody] AddFriendDto dto)
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            ResultError? error = await _userFriendService.AcceptFriendRequest(dto.UserId, userId);
            if (error is null) return Created();
            return ControllerUtils.OutputErrorResult(error);
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromFriends([FromBody] AddFriendDto dto)
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            ResultError? error = await _userFriendService.RemoveFromFriends(userId, dto.UserId);
            if (error is null) return Ok();
            return ControllerUtils.OutputErrorResult(error);
        }
    }
}
