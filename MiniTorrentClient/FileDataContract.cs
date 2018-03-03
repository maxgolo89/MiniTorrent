using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrent_GUI
{
    class FileDataContract
    {
        public string Filename { get; set; }
        public int StartByte { get; set; }
        public int EndByte { get; set; }
        public int TotalFileSizeInBytes { get; set; }
    }
}
