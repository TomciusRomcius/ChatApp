namespace ChatApp.Domain.Entities
{
    public class TextMessageEntity
    {
        public Guid TextMessageId { get; set; }
        public required string Content { get; set; }
    }
}