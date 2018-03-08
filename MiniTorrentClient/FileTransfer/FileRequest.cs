using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentClient.FileTransfer
{
    class FileRequest
    {
        public string Name { get; set; }
        public int StartByte { get; set; }
        public int EndByte { get; set; }
        public int Size { get; set; }
    }
}
