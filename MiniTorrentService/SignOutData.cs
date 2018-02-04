using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentService
{
    [DataContract]
    public class SignOutData
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "password")]
        public string Password { get; set; }

        public SignOutData(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public override string ToString()
        {
            return Username + " " + Password;
        }
    }
}
