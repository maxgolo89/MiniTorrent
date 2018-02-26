using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentDAL.Messeges
{
    public class FileObject
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public List<LoggedInUserObject> Resources { get; set; }
    }
}
