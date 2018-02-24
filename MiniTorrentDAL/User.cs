using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentDAL
{
    public class User
    {
        [Key]
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public bool IsAdmin { get; set; }

        public User()
        {
            IsAdmin = false;
        }
    }
}
