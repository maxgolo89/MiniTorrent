using System.ComponentModel.DataAnnotations;

namespace MiniTorrentDAL
{

    public class UserEntity
    {
        [Key]
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}