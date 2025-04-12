using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities.ChatRoom
{
    public class ChatRoomEntity
    {
        public required string ChatRoomId { get; set; }
        public required string Name { get; set; }
        public required string AdminUserId { get; set; }
        public IdentityUser? AdminUser { get; set; }

        public List<ResultError> Validate()
        {
            var errors = new List<ResultError>();
            if (string.IsNullOrWhiteSpace(Name)) errors.Add(new ResultError(ResultErrorType.VALIDATION_ERROR, "Name cannot be empty"));
            if (string.IsNullOrWhiteSpace(ChatRoomId)) errors.Add(new ResultError(ResultErrorType.VALIDATION_ERROR, "ChatRoomId cannot be empty"));
            if (string.IsNullOrWhiteSpace(AdminUserId)) errors.Add(new ResultError(ResultErrorType.VALIDATION_ERROR, "AdminUserId cannot be empty"));
            
            return errors;
        }
    }
}