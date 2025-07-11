using System.ComponentModel.DataAnnotations;

namespace ZOV.Models
{
    public class ConnectedUser
    {
        public string? ConnectionID { get; set; }
        public string? Login { get; set; }
    }
}
