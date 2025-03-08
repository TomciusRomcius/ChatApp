using System.ComponentModel.DataAnnotations;

namespace ChatApp.Dtos
{
    public class AuthenticateWithOidcDto
    {
        [Required(AllowEmptyStrings = false)]
        public string Provider { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string AuthorizationCode { get; set; }
    }
}