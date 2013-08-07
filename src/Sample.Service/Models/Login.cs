using System.ComponentModel.DataAnnotations;

namespace TS.FormsToTokenAccessAuthentication.Sample.Service.Models
{
    public class Login
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}