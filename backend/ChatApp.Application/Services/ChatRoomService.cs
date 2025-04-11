using ChatApp.Application.Persistance;
using ChatApp.Domain.Entities.ChatRoom;
using ChatApp.Domain.Utils;
using ChatApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Services
{
    public class ChatRoomService : IChatRoomService
    {
        readonly DatabaseContext _databaseContext;

        public ChatRoomService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Result<List<ChatRoomEntity>> GetChatRooms(string userId)
        {
            if (userId.Length == 0)
            {
                return new Result<List<ChatRoomEntity>>([new ResultError(ResultErrorType.VALIDATION_ERROR, "User id is empty")]);
            }

            var query = from chatroomMember in _databaseContext.ChatRoomMembers
                        where chatroomMember.MemberId == userId
                        join chatroom in _databaseContext.ChatRooms
                        on chatroomMember.ChatRoomId equals chatroom.ChatRoomId
                        select chatroom;

            List<ChatRoomEntity> result = [.. query];

            return new Result<List<ChatRoomEntity>>(result);
        }

        public async Task<Result<string>> CreateChatRoomAsync(string adminUserId, string chatRoomName, List<string> members)
        {

            var chatroom = new ChatRoomEntity
            {
                ChatRoomId = Guid.NewGuid().ToString(),
                AdminUserId = adminUserId,
                Name = chatRoomName,
            };

            await _databaseContext.ChatRooms.AddAsync(chatroom);

            // If user provides initial chat room members, add them 
            // TODO: check if all members are friends
            if (members.Any())
            {
                ChatRoomMemberEntity adminMember = new ChatRoomMemberEntity
                {
                    ChatRoomId = chatroom.ChatRoomId,
                    MemberId = adminUserId,
                };

                await _databaseContext.ChatRoomMembers.AddRangeAsync(
                    [..members.Select(uid => new ChatRoomMemberEntity
                    {
                        ChatRoomId = chatroom.ChatRoomId,
                        MemberId = uid
                    }),
                    adminMember
                    ]
                );

                await _databaseContext.SaveChangesAsync();
            }

            return new Result<string>(chatroom.ChatRoomId);
        }

        public async Task<ResultError?> DeleteChatRoomAsync(string userId, string chatRoomId)
        {
            if (userId.Length == 0)
            {
                return new ResultError(ResultErrorType.VALIDATION_ERROR, "User id is empty");
            }

            if (chatRoomId.Length == 0)
            {
                return new ResultError(ResultErrorType.VALIDATION_ERROR, "Chat room id is empty");
            }

            var query = _databaseContext.ChatRooms.Where(cr => cr.ChatRoomId == chatRoomId);
            ChatRoomEntity? chatRoom = query.FirstOrDefault();

            if (chatRoom is null)
            {
                return new ResultError(ResultErrorType.VALIDATION_ERROR, "Chat room does not exist");
            }

            if (userId != chatRoom.AdminUserId)
            {
                return new ResultError(ResultErrorType.FORBIDDEN_ERROR, "Trying to delete a chat room while not being an admin");
            }

            await query.ExecuteDeleteAsync();

            return null;
        }
    }
}