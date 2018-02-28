using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MiniTorrentService.RequestData;

namespace MiniTorrentService
{
    [DataContract]
    public class UserListPerFileRequest : Request
    {
        [DataMember(Name = "filename")]
        public string FileName { get; set; }
    }
}
