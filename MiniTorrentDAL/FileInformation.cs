using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentDAL
{
    public class FileInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Size { get; set; }

        [ForeignKey("LoggedInUser")]
        [Required]
        public string SessionId { get; set; }
        public virtual List<LoggedInUser> LoggedInUser { get; set; }
    }
}
