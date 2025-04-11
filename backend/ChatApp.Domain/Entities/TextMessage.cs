using ChatApp.Domain.Entities.ChatRoom;
using ChatApp.Domain.Utils;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities
{
    /*
        Not the most efficient way in terms of storage but don't want to have a
        separate table that maps each message type to usermessage or chatroom message
    */
    public class TextMessageEntity
    {
        public required string TextMessageId { get; set; }
        public required string Content { get; set; }
        public required string SenderId { get; set; }
        public string? ChatRoomId { get; set; }
        public string? ReceiverUserId { get; set; }
        public ChatRoomEntity? ChatRoom { get; set; }
        public IdentityUser? Sender { get; set; }
        public IdentityUser? ReceiverUser { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<ResultError> Validate()
        {
            var errors = new List<ResultError>();

            if (string.IsNullOrWhiteSpace(TextMessageId)) errors.Add(new ResultError(ResultErrorType.VALIDATION_ERROR, "TextMessageId cannot be empty"));
            if (string.IsNullOrWhiteSpace(Content)) errors.Add(new ResultError(ResultErrorType.VALIDATION_ERROR, "Content cannot be empty"));
            if (string.IsNullOrWhiteSpace(SenderId)) errors.Add(new ResultError(ResultErrorType.VALIDATION_ERROR, "SenderId cannot be empty"));
            if (string.IsNullOrEmpty(ReceiverUserId) && string.IsNullOrEmpty(ChatRoomId))
            {
                errors.Add(new ResultError(ResultErrorType.VALIDATION_ERROR, "You must define a ReceiverUserId or ChatRoomId"));
            }
            if (!string.IsNullOrEmpty(ReceiverUserId) && !string.IsNullOrEmpty(ChatRoomId))
            {
                errors.Add(new ResultError(ResultErrorType.VALIDATION_ERROR, "You cannot specify both ReceiverUserId and ChatRoomId"));
            }
            if (CreatedAt == default)
            {
                errors.Add(new ResultError(ResultErrorType.VALIDATION_ERROR, "CreatedAt must be set"));
            }

            return errors;
        }
    }
}