using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniTorrentDAL
{
    public class LoggedInUserEntity
    {
        [Key]
        [ForeignKey("User")]
        [Required]
        public string Username { get; set; }
        public UserEntity User { get; set; }

        [Required]
        public string Ip { get; set; }
        
        [Required]
        public int Port { get; set; }

        public virtual ICollection<FileEntity> Files { get; set; }
    }
}