using System.ComponentModel.DataAnnotations;
using ChatApp.Domain.Utils;

namespace ChatApp.Presentation.Controllers.UserFriend.Dtos;

public class GetUserRelationshipsDto
{
    [Required] public short Status { get; set; }

    [Required] public required UserRelationshipType RelationshipType { get; set; }
}