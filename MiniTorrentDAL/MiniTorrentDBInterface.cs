using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MiniTorrentDAL
{
    public class MiniTorrentDBInterface : IMiniTorrentDBInterface
    {
        public bool InsertNewUser(User user)
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {
                    var query = db.Users.Find(user.Username);
                    if (query != null)
                    {
                        return false;
                    }
                    db.Users.Add(user);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    ExceptionLogger(e);
                }
            }
            return false;
        }

        public bool InsertUserToLoggedInUser(User user, string sessionId, DateTime timeStamp, string ip, int port, List<FileInformation> files)
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {
                    // Authenticate user
                    UserAuthentication(user, db);

                    // Remove user from LoggedInUsers if present
                    RemoveLoggedInUserHelper(user, db);

                    // Add entry to loggedinusers
                    db.LoggedInUsers.Add(new LoggedInUser()
                    {
                        Username = user.Username,
                        SessionId = sessionId,
                        TimeStamp = timeStamp,
                        IP = ip,
                        Port = port,
                        Files = files
                    });
                    
                    db.SaveChanges();

                    return true;
                }
                catch (Exception e)
                {
                    ExceptionLogger(e);
                }
                return false;
            }
        }

        public bool RemoveUserFromLoggedInUser(string sessionId)
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {
                    var query = db.LoggedInUsers.Find(sessionId);
                    if (query != null)
                    {
                        db.LoggedInUsers.Remove(query);
                        db.SaveChanges();
                        return true;
                    }  
                    return false;
                }
                catch (Exception e)
                {
                    ExceptionLogger(e);
                }

                return false;
            }
        }

        public bool RemoveUserFromLoggedInUser(User user)
        {
            using (var db = new MiniTorrentContext())
            {
                if(UserAuthentication(user, db))
                    return RemoveLoggedInUserHelper(user, db);
                return false;
            }
        }

        public FileInformation GetFile(string name)
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {
                    
                }
                catch (Exception e)
                {
                    ExceptionLogger(e);
                }
                return new FileInformation();
            }
        }

        public List<FileInformation> GetAllFiles()
        {
            throw new NotImplementedException();
        }

        public bool UpdateFileList(string sessionId, List<FileInformation> files)
        {
            throw new NotImplementedException();
        }

        public bool UpdateFileList(User user, List<FileInformation> files)
        {
            throw new NotImplementedException();
        }

        public bool UpdateTimeStamp(string sessionId, DateTime timeStamp)
        {
            throw new NotImplementedException();
        }
        private void ExceptionLogger(Exception e)
        {
            StringBuilder output = new StringBuilder();
            output.Append("[" + DateTime.Now + "] ");
            output.Append(e.Message + "\n");
            output.Append(e.InnerException + "\n");

            Console.Write(output.ToString());
        }

        private bool RemoveLoggedInUserHelper(User user, MiniTorrentContext db)
        {
            try
            {
                var loggedUserQuery = from loggedUser in db.LoggedInUsers where loggedUser.Username == user.Username select loggedUser;

                if (loggedUserQuery == null)
                    return false;

                
                foreach (var lu in loggedUserQuery)
                {
                    
                    db.LoggedInUsers.Remove(lu);
                    
                }
                
                db.SaveChanges();
                return true;

            }
            catch (Exception e)
            {
                ExceptionLogger(e);
            }
            return false;
        }

        private bool UserAuthentication(User user, MiniTorrentContext db)
        {
            try
            {
                // Check if user exists in users table
                var userQuery = db.Users.Find(user.Username);
                if (userQuery == null)
                    return false;

                // Test password
                if (userQuery.Password != user.Password)
                    return false;

                return true;
            }
            catch (Exception e)
            {
                ExceptionLogger(e);
            }

            return false;
        }
    }
}
