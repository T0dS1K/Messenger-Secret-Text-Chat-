using System.ComponentModel.DataAnnotations;

namespace ZOV.Models
{
    public class NewUser
    {
        public string? Login { get; set; }
        public string? Password { get; set; }
    }
}
