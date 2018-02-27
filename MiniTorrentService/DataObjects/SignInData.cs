using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentService
{
    [DataContract]
    public class SignInData
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "password")]
        public string Password { get; set; }
        [DataMember(Name = "ip")]
        public string IP { get; set; }
        [DataMember(Name = "port")]
        public string Port { get; set; }
        [DataMember(Name = "files")]
        public Dictionary<string, int> Files { get; set; }

        public SignInData(string username, string password, string ip, string port)
        {
            Username = username;
            Password = password;
            IP = ip;
            Port = port;
        }

        public override string ToString()
        {
            return this.Username + " " + this.Password + " " + this.IP + " " + this.Port + " " + this.Files.ToString();
        }
    }
}
