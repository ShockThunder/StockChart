using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorBattles.Shared
{
    public class UserRegister
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        
        [StringLength(16, ErrorMessage = "Too long username (16 characters max)")]
        public string Username { get; set; }
        
        public string Bio { get; set; }

        [Required, StringLength(100, MinimumLength = 10)]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "passwords doesn't match")]
        public string ConfirmPassword { get; set; }
        public int StartUnitId { get; set; } = 1;
        
        [Range(0, 1000, ErrorMessage = "please choose between 0 and 1000")]
        public int Bananas { get; set; } = 100;

        public DateTime BirthDate { get; set; } = DateTime.Now;
        
        [Range(typeof(bool), "true", "true", ErrorMessage = "Only confirmed players can play")]
        public bool IsConfirmed { get; set; } = true;
    }
}
