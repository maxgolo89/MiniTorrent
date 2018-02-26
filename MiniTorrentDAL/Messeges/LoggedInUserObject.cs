using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentDAL.Messeges
{
    public class LoggedInUserObject
    {
        public string Username { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public List<FileObject> files { get; set; }
    }
}
