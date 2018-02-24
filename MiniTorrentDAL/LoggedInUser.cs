using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Net;
using System.Net.NetworkInformation;
using System.Web.Services.Description;

namespace MiniTorrentDAL
{
    public class LoggedInUser
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public User User { get; set; }
        [Key]
        [Required]
        public string SessionId { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        [Required]
        public string IP { get; set; }
        [Required]
        public int Port { get; set; }
        [Required]
        public virtual List<FileInformation> Files { get; set; }
    }
}