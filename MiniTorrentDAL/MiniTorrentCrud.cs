using System;
using System.Collections.Generic;
using System.Linq;
using MiniTorrentDAL.Messeges;

namespace MiniTorrentDAL
{
    public class MiniTorrentCrud : IMiniTorrentCrud
    {
        /// <summary>
        /// Create a user entry in the database.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>success - usename, failure - null</returns>
        public string CreateUser(string username, string password)
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {
                    UserEntity newUser = new UserEntity();
                    newUser.Username = username;
                    newUser.Password = password;
                    db.UserEntiries.Add(newUser);
                    db.SaveChanges();
                    ConsoleLogger("User " + username + " entry was added to users");
                    return username;
                }
                catch (Exception e)
                {
                    ConsoleLogger(e.Message);
                }

                return null;
            }
        }

        /// <summary>
        /// Create a loggedinuser entry in the database.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="files"></param>
        /// <returns>success - usename, failure - null</returns>
        public string CreateLoggedInUser(string username, string ip, int port, Dictionary<string, int> files)
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {
                    LoggedInUserEntity loggedInUserEntity = new LoggedInUserEntity();
                    List<FileEntity> fileEntities = new List<FileEntity>();


                    // Manage file-logged user relationship
                    foreach (var file in files)
                    {
                        var fileQuery = db.FileEntities.Find(file.Key);
                        if (fileQuery != null)
                        {
                            fileQuery.Resources.Add(loggedInUserEntity);
                            fileEntities.Add(fileQuery);
                        }
                        else
                        {
                            FileEntity fileEntity = new FileEntity();
                            fileEntity.Name = file.Key;
                            fileEntity.Size = file.Value;
                            fileEntity.Resources = new List<LoggedInUserEntity>();
                            fileEntity.Resources.Add(loggedInUserEntity);
                            fileEntities.Add(fileEntity);
                        }
                    }

                    // Logged user initialization
                    loggedInUserEntity.Username = username;
                    loggedInUserEntity.Ip = ip;
                    loggedInUserEntity.Port = port;
                    loggedInUserEntity.Files = fileEntities;


                    // Save to db
                    db.LoggedInUserEntities.Add(loggedInUserEntity);
                    db.SaveChanges();
                    ConsoleLogger(username + " " + ip + " " + port + " " + "entry was added to loggedinusers");
                    return username;
                }
                catch (Exception e)
                {
                    ConsoleLogger(e.Message);
                }

                return null;
            }
        }

        /// <summary>
        /// Update file list, for loggedin user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="files"></param>
        /// <returns>success - usename, failure - null</returns>
        public string UpdateFilesForLoggedInUser(string username, Dictionary<string, int> files)
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {
                    // Query for the user
                    var loggedInUserQuery = db.LoggedInUserEntities.Find(username);
                    if (loggedInUserQuery == null)
                        return null;

                    // Traverse each file in the given dectionary.
                    foreach (var file in files)
                    {
                        // Search user file list for current file.
                        // If file found, stop searching and set flag to true.
                        bool flag = false;
                        foreach (var fileEntity in loggedInUserQuery.Files)
                        {
                            if (fileEntity.Name == file.Key)
                            {
                                flag = true;
                                break;
                            }
                        }

                        // If flag is true, skip to next iteration.
                        // else perform clause.
                        if (!flag)
                        {
                            // Search file in file entities.
                            var fileQuery = db.FileEntities.Find(file.Key);
                            if (fileQuery != null)
                            {
                                fileQuery.Resources.Add(loggedInUserQuery);
                                loggedInUserQuery.Files.Add(fileQuery);
                            }
                            else
                            {
                                // Create new FileEntity
                                FileEntity temp = new FileEntity();
                                temp.Name = file.Key;
                                temp.Size = file.Value;
                                temp.Resources = new List<LoggedInUserEntity>();
                                temp.Resources.Add(loggedInUserQuery);
                                loggedInUserQuery.Files.Add(temp);
                            }
                        }
                    }
                    db.SaveChanges();
                    ConsoleLogger("Files for user: " + username + " were updated");
                    return username;
                }
                catch (Exception e)
                {
                    ConsoleLogger(e.Message);
                }

                return null;
            }
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string UpdateLoggedInUserTimeStamp(string username)
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {

                }
                catch (Exception e)
                {
                    ConsoleLogger(e.Message);
                }

                return null;
            }
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <returns></returns>
        public List<UserObject> ReadUsers()
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {
                    
                }
                catch (Exception e)
                {
                    ConsoleLogger(e.Message);
                }

                return null;
            }
        }
        
        /// <summary>
        /// Get a user from database.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Success - UserObject, Failure - null</returns>
        public UserObject ReadUser(string username)
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {
                    var userQuery = db.UserEntiries.Find(username);
                    if (userQuery == null)
                        return null;

                    UserObject temp = new UserObject();
                    temp.Username = userQuery.Username;
                    temp.Password = userQuery.Password;
                    ConsoleLogger(username + " was retrieved from database.");
                    return temp;
                }
                catch (Exception e)
                {
                    ConsoleLogger(e.Message);
                }

                return null;
            }
        }

        /// <summary>
        /// Get all available files in database.
        /// </summary>
        /// <returns>Success - List of FileObject, Failure - null</returns>
        public List<FileObject> ReadFiles()
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {
                    List<FileObject> fileList = new List<FileObject>();
                    var filesQuery = db.FileEntities.ToList();

                    foreach (var fileEntity in filesQuery)
                    {
                        // Fetch owners.
                        List<LoggedInUserObject> userList = new List<LoggedInUserObject>();
                        List<LoggedInUserEntity> loggedInUserEntities = fileEntity.Resources.ToList();

                        foreach (var userEntity in loggedInUserEntities)
                        {
                            LoggedInUserObject temp = new LoggedInUserObject();
                            temp.Username = userEntity.Username;
                            temp.Ip = userEntity.Ip;
                            temp.Port = userEntity.Port;
                            userList.Add(temp);
                        }

                        // Pack into FileObject.
                        FileObject tempFileObject = new FileObject();
                        tempFileObject.Name = fileEntity.Name;
                        tempFileObject.Size = fileEntity.Size;
                        tempFileObject.Resources = userList;

                        // Add to list.
                        fileList.Add(tempFileObject);
                    }
                    ConsoleLogger("Retrieved file list");
                    return fileList;
                }
                catch (Exception e)
                {
                    ConsoleLogger(e.Message);
                }

                return null;
            }
        }

        /// <summary>
        /// Get file information from database.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Success - FileObject, Failure - null</returns>
        public FileObject ReadFile(string name)
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {
                    var fileQuery = db.FileEntities.Find(name);
                    if (fileQuery == null)
                        return null;
                    FileObject temp = new FileObject();
                    temp.Name = fileQuery.Name;
                    temp.Size = fileQuery.Size;
                    temp.Resources = new List<LoggedInUserObject>();

                    foreach (var user in fileQuery.Resources)
                    {
                        LoggedInUserObject tempUserObject = new LoggedInUserObject();
                        tempUserObject.Username = user.Username;
                        tempUserObject.Ip = user.Ip;
                        tempUserObject.Port = user.Port;
                        temp.Resources.Add(tempUserObject);
                    }

                    return temp;
                }
                catch (Exception e)
                {
                    ConsoleLogger(e.Message);
                }

                return null;
            }
        }
        
        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <returns></returns>
        public List<LoggedInUserObject> ReadLoggedInUsers()
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {

                }
                catch (Exception e)
                {
                    ConsoleLogger(e.Message);
                }

                return null;
            }
        }

        /// <summary>
        /// Get a loggedinuser from database.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Success - LoggedInUserObject, Failure - null</returns>
        public LoggedInUserObject ReadLoggedInUser(string username)
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {
                    var loggedInUserQuery = db.LoggedInUserEntities.Find(username);
                    var loggedInUserObject = new LoggedInUserObject();

                    if (loggedInUserQuery == null)
                        return null;

                    loggedInUserObject.Username = loggedInUserQuery.Username;
                    loggedInUserObject.Ip = loggedInUserQuery.Ip;
                    loggedInUserObject.Port = loggedInUserQuery.Port;

                    List<FileObject> fileList = new List<FileObject>();
                    foreach (var file in loggedInUserQuery.Files)
                    {
                        FileObject temp = new FileObject();
                        temp.Name = file.Name;
                        temp.Size = file.Size;
                        fileList.Add(temp);
                    }

                    loggedInUserObject.files = fileList;

                    return loggedInUserObject;
                }
                catch (Exception e)
                {
                    ConsoleLogger(e.Message);
                }

                return null;
            }
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string DeleteUser(string username)
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {

                }
                catch (Exception e)
                {
                    ConsoleLogger(e.Message);
                }

                return null;
            }
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool DeleteUsers()
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {

                }
                catch (Exception e)
                {
                    ConsoleLogger(e.Message);
                }

                return false;
            }
        }

        /// <summary>
        /// Delete loggedinuser from database.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Success - username, Failure - null</returns>
        public string DeleteLoggedInUser(string username)
        {
            
            using (var db = new MiniTorrentContext())
            {
                try
                {
                    // Query for the user in loggedinusers
                    var loggedInUserQuery = db.LoggedInUserEntities.Find(username);
                    if (loggedInUserQuery == null)
                        return null;

                    // List all files of the found user.
                    List<FileEntity> files = loggedInUserQuery.Files.ToList();
                    db.LoggedInUserEntities.Remove(loggedInUserQuery);

                    // For each file the user had, if he is the only resource, remove from file table.
                    foreach (var file in files)
                    {
                        if (file.Resources.Count == 0)
                            db.FileEntities.Remove(file);
                    }

                    db.SaveChanges();
                    ConsoleLogger(username + " entry was removed from loggedinuser table.");
                    return username;
                }
                catch (Exception e)
                {
                    ConsoleLogger(e.Message);
                }

                return null;
            }
        }

        /// <summary>
        /// NOT IMPLEMENTED.
        /// </summary>
        /// <returns></returns>
        public bool DeleteLoggedInUsers()
        {
            using (var db = new MiniTorrentContext())
            {
                try
                {

                }
                catch (Exception e)
                {
                    ConsoleLogger(e.Message);
                }

                return false; ;
            }
        }

        private void ConsoleLogger(string messege)
        {
            Console.WriteLine("[" + DateTime.Now + "] " + messege);
        }
    }
}