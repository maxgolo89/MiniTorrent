using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentClient
{
    class Configuration
    {
        public string ServerAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string HostIp { get; set; }
        public int InPort { get; set; }
        public int OutPort { get; set; }
        public string SharedFolder { get; set; }
        public string DestinationFolder { get; set; }
    }
}
