namespace ChatApp.Domain.Models;

public class UserTextMessageModel
{
    public UserTextMessageModel(string textMessageId, string senderId, string content, DateTime createdAt)
    {
        TextMessageId = textMessageId;
        SenderId = senderId;
        Content = content;
        CreatedAt = createdAt;
    }

    public string TextMessageId { get; set; }
    public string SenderId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; }
}