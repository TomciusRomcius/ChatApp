namespace ChatApp.Domain.Models
{
    public class UserTextMessageModel
    {
        public Guid TextMessageId { get; set; }
        public string SenderId { get; set; }
        public string Sender { get; set; }
    }
}