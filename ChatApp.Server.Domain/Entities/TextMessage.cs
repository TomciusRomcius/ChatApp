namespace ChatApp.Domain.Entities.ChatRoomMessage
{
    public class TextMessageEntity
    {
        public required Guid TextMessageId { get; set; }
        public required string Content { get; set; }
    }
}