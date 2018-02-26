using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniTorrentDAL
{
    public class FileEntity
    {
        [Key]
        [Required]
        public string Name { get; set; }

        [Required]
        public int Size { get; set; }

        
        public virtual ICollection<LoggedInUserEntity> Resources { get; set; }
    }
}