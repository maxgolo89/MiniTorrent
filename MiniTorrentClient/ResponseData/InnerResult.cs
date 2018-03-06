using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MiniTorrentClient.ResponseData
{
    public class InnerResult
    {
        public InnerResponse response { get; set; }
    }

    public class InnerResponse
    {
        public string username { get; set; }
    }
}
