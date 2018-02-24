using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MiniTorrentDAL;

namespace MiniTorrentClientTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            MiniTorrentDAL.Test.DatabaseTest();
            Console.WriteLine("Done!");
            Console.ReadKey();
        }


        public static void RestTest()
        {
            WebClient request = new WebClient();
            Uri httpUri = new Uri("http://localhost:8090/MiniTorrentService/signIn");
            Uri httpUri2 = new Uri("http://localhost:8090/MiniTorrentService/signOut");
            Uri httpUri3 = new Uri("http://localhost:8090/MiniTorrentService/request");


            object signInDataJson = new
            {
                username = "popo",
                password = "password",
                ip = "10.10.10.10",
                port = "9999",
                files = new Dictionary<string, int> { { "file_1", 123456789 }, { "file_2", 987654321 } }

            };

            object signOutDataJson = new
            {
                username = "popo",
                password = "password"
            };

            object requestDataJson = new
            {
                username = "popo",
                password = "password",
                file = "file_1"
            };




            string signInSerialize = (new JavaScriptSerializer()).Serialize(new
            {
                signInData = signInDataJson
            });

            string signOutSerialize = (new JavaScriptSerializer()).Serialize(new
            {
                signOutData = signOutDataJson
            });

            string requestSerialize = (new JavaScriptSerializer()).Serialize(new
            {
                requestData = requestDataJson
            });

            request.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            request.UploadString(httpUri, "POST", signInSerialize);

            request.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            request.UploadString(httpUri2, "POST", signOutSerialize);

            request.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            request.UploadString(httpUri3, "POST", requestSerialize);


            Console.WriteLine("Requests sent");
        }
    }
}
