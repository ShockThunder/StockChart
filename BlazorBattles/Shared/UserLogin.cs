using System.ComponentModel.DataAnnotations;

namespace BlazorBattles.Shared
{
    public class UserLogin
    {
        [Required(ErrorMessage ="Please, enter username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please, enter password")]
        public string Password { get; set; }
    }
}
