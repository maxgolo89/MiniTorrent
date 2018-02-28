using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using MiniTorrentService.RequestData;

namespace MiniTorrentService
{
    [DataContract]
    public class LoginRequest : Request
    {
        [DataMember(Name = "ip")]
        public string IP { get; set; }

        [DataMember(Name = "port")]
        public int Port { get; set; }

        [DataMember(Name = "files")]
        public Dictionary<string, int> Files { get; set; }
    }
}
