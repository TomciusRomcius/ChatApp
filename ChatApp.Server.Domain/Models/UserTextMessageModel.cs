namespace ChatApp.Domain.Models
{
    public class UserTextMessageModel
    {
        public Guid TextMessageId { get; set; }
        public string SenderId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; }

        public UserTextMessageModel(Guid textMessageId, string senderId, string content, DateTime createdAt)
        {
            TextMessageId = textMessageId;
            SenderId = senderId;
            Content = content;
            CreatedAt = createdAt;
        }
    }
}