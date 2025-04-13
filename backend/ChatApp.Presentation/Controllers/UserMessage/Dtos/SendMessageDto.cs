namespace ChatApp.Presentation.UserFriend;

public class SendMessageDto
{
    public required string ReceiverId { get; set; }
    public required string Content { get; set; }
}