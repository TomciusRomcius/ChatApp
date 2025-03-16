using System.Security.Claims;
using ChatApp.Application.Persistance;
using ChatApp.Domain.Entities.ChatRoom;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Server.Presentation.ChatRoom
{
    [ApiController]
    [Route("[controller]")]
    public class ChatRoomController : ControllerBase
    {
        readonly DatabaseContext _databaseContext;

        public ChatRoomController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        [HttpGet]
        public IActionResult GetChatRooms()
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            var query = from chatroomMember in _databaseContext.ChatRoomMembers
                        where chatroomMember.MemberId == userId
                        join chatroom in _databaseContext.ChatRooms
                        on chatroomMember.ChatRoomId equals chatroom.ChatRoomId
                        select chatroom;

            List<ChatRoomEntity> result = [.. query];

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChatRoomAsync([FromBody] CreateChatRoomDto dto)
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            var chatroom = new ChatRoomEntity
            {
                ChatRoomId = Guid.NewGuid().ToString(),
                AdminUserId = userId,
                Name = dto.Name,
            };

            await _databaseContext.ChatRooms.AddAsync(chatroom);

            // If user provides initial chat room members, add them 
            if (dto.Members.Any())
            {
                await _databaseContext.ChatRoomMembers.AddRangeAsync(dto.Members.Select(uid => new ChatRoomMemberEntity
                {
                    ChatRoomId = chatroom.ChatRoomId,
                    MemberId = uid
                }));

                await _databaseContext.SaveChangesAsync();
            }

            return Created("", new { ChatRoomId = chatroom.ChatRoomId });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteChatRoomAsync([FromBody] DeleteChatRoomDto dto)
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            var query = _databaseContext.ChatRooms.Where(cr => cr.ChatRoomId == dto.ChatRoomId);
            ChatRoomEntity? chatRoom = query.FirstOrDefault();

            if (chatRoom is null)
            {
                return BadRequest("Chat room does not exist");
            }

            if (userId != chatRoom.AdminUserId)
            {
                return Forbid();
            }

            await query.ExecuteDeleteAsync();

            return Created();
        }
    }
}