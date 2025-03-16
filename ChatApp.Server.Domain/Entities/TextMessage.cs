namespace ChatApp.Server.Domain.Entities
{
    public class TextMessageEntity
    {
        public string TextMessageId { get; set; }
        public required string Content { get; set; }
    }
}