using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities.ChatRoom;

public class ChatRoomMemberEntity
{
    public required string ChatRoomId { get; set; }
    public required string MemberId { get; set; }
    public ChatRoomEntity? ChatRoom { get; set; }
    public IdentityUser? Member { get; set; }
}