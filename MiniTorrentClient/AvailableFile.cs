using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MiniTorrentClient
{
    class AvailableFile
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public List<TorrentUser> Owners { get; set; }
    }
}
