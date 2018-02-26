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
using MiniTorrentDAL.Messeges;

namespace MiniTorrentClientTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            MiniTorrentCrud db = new MiniTorrentCrud();
//            DALCreateUserTest(db);
//            DALCreateLoggedInUserTest(db);
//            DALDeleteLoggedInUserTest(db);
//            DALUpdateFileList(db);
//            DALUpdateFileList2(db);
//            DALReadUser(db);
//            DALReadFiles(db);
//            DALReadFile(db);
//            DALReadLoggedInUser(db);
            Console.ReadKey();
        }


        public static void DALCreateUserTest(MiniTorrentCrud db)
        {
            
            db.CreateUser("u1", "u1");
            db.CreateUser("u2", "u2");
            db.CreateUser("u3", "u3");
            db.CreateUser("u4", "u4");
        }

        public static void DALCreateLoggedInUserTest(MiniTorrentCrud db)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dict.Add("SOSO", 10000);
            dict.Add("POPO", 20000);
            dict.Add("TOTO", 30000);

            db.CreateLoggedInUser("u1", "10.10.10.10", 9999, dict);
            db.CreateLoggedInUser("u2", "10.10.10.11", 9999, dict);
        }

        public static void DALDeleteLoggedInUserTest(MiniTorrentCrud db)
        {
            db.DeleteLoggedInUser("u1");
            db.DeleteLoggedInUser("u2");
        }

        public static void DALUpdateFileList(MiniTorrentCrud db)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dict.Add("SOSO", 10000);
            dict.Add("POPO", 20000);
            dict.Add("TOTO", 30000);
            dict.Add("MOMO", 40000);
            dict.Add("KOKO", 50000);
            db.UpdateFilesForLoggedInUser("u1", dict);
        }

        public static void DALUpdateFileList2(MiniTorrentCrud db)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dict.Add("KOKO", 50000);
            db.UpdateFilesForLoggedInUser("u2", dict);
        }

        public static void DALReadUser(MiniTorrentCrud db)
        {
            Console.WriteLine(db.ReadUser("u1"));
        }

        public static void DALReadFiles(MiniTorrentCrud db)
        {
            List<FileObject> list = db.ReadFiles();
            if (list != null)
            {
                foreach (var file in list)
                {
                    Console.WriteLine(file.Name);
                    Console.WriteLine("**************");
                    foreach (var resource in file.Resources)
                    {
                        Console.WriteLine(resource.Username);
                    }

                    Console.WriteLine();
                }
            }
        }

        public static void DALReadFile(MiniTorrentCrud db)
        {
            FileObject file = db.ReadFile("SOSO");
            if (file != null)
            {
                Console.WriteLine(file.Name);
                Console.WriteLine("**************");
                foreach (var resource in file.Resources)
                {
                    Console.WriteLine(resource.Username);
                }
            }
        }

        public static void DALReadLoggedInUser(MiniTorrentCrud db)
        {
            LoggedInUserObject loggedInUserObject = db.ReadLoggedInUser("u1");
            if (loggedInUserObject != null)
            {
                Console.WriteLine(loggedInUserObject.Username);
                Console.WriteLine("**************");
                foreach (var file in loggedInUserObject.files)
                {
                    Console.WriteLine(file.Name);
                }
            }
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
