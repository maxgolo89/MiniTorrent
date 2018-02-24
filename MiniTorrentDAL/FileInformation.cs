using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentDAL
{
    public class FileInformation
    {
        [Key]
        [Required]
        public string Name { get; set; }
        public int Size { get; set; }
        [Required]
        public virtual List<LoggedInUser> Seeds { get; set; }
    }
}
