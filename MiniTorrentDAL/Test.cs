using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MiniTorrentDAL
{
    public class Test
    {
        public static void DatabaseTest()
        {
            using (var db = new MiniTorrentContext())
            {
                User u1 = new User();
                u1.Username = "u1";
                u1.Password = "p1";

                User u2 = new User();
                u2.Username = "u2";
                u2.Password = "p2";

                User u3 = new User();
                u3.Username = "u3";
                u3.Password = "p3";

                try
                {
                    db.Users.Add(u1);
                    db.Users.Add(u2);
                    db.Users.Add(u3);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                try
                {
                    var users = db.Users;

                    foreach (User user in users)
                    {
                        Console.WriteLine(user.Username);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.InnerException);
                }
                
            }
        }
    }
}
