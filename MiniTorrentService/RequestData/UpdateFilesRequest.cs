using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentService.RequestData
{
    [DataContract]
    public class UpdateFilesRequest : Request
    {
        [DataMember(Name = "files")]
        public Dictionary<string, int> Files { get; set; }
    }
}
