namespace ChatApp.Server.Domain.Entities
{
    public class TextMessageEntity
    {
        public required string TextMessageId { get; set; }
        public required string Content { get; set; }
    }
}