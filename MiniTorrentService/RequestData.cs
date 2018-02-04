using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentService
{
    [DataContract]
    public class RequestData
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "password")]
        public string Password { get; set; }
        [DataMember(Name = "file")]
        public string FileName { get; set; }

        public RequestData(string username, string password, string fileName)
        {
            Username = username;
            Password = password;
            FileName = fileName;
        }

        public override string ToString()
        {
            return Username + " " + Password + " " + FileName;
        }
    }
}
