using System.ComponentModel.DataAnnotations;

namespace ZOV.Models
{
    public class User
    {
        [Key]
        [Required]
        [StringLength(10)]
        public string? Login { get; set; }

        [Required]
        [StringLength(60)]
        public string? Password { get; set; }

        [Required]
        [StringLength(30)]
        public string? Salt { get; set; }
    }
}
