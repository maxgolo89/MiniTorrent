﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentService.RequestData
{
    [DataContract]
    public class Request
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }
    }
}