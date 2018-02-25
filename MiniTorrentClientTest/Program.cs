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
            DALTest();
        }


        public static void DALTest()
        {
            MiniTorrentDBInterface dal = new MiniTorrentDBInterface();

            // *********************************************************************************
            // Test InsertNewUser
            // *********************************************************************************
            User u1 = new User();
            u1.Username = "first";
            u1.Password = "first";
            var success = dal.InsertNewUser(u1);
            if(success)
                Console.WriteLine("Yay!");
            else
            {
                Console.WriteLine("Bummer...");
            }

            // *********************************************************************************
            // Test InsertUserToLoggedInUser
            // *********************************************************************************
            List<FileInformation> files = new List<FileInformation>();
            FileInformation file = new FileInformation();
            file.Name = "SOSO";
            file.Size = 100000;
            files.Add(file);
            var sid = DateTime.Now.ToBinary().ToString();

            foreach (var f in files)
            {
                f.SessionId = sid;
            }

            success = dal.InsertUserToLoggedInUser(u1, DateTime.Now.ToBinary().ToString(), DateTime.Now, "10.0.0.100", 45454,
                files);

            if (success)
                Console.WriteLine("Yay!");
            else
            {
                Console.WriteLine("Bummer...");
            }

            // *********************************************************************************
            // Test RemoveLoggedInUser
            // *********************************************************************************
//            success = dal.RemoveUserFromLoggedInUser(u1);
            if (success)
                Console.WriteLine("Yay!");
            else
            {
                Console.WriteLine("Bummer...");
            }

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
