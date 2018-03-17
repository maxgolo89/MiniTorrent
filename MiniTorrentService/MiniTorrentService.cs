using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MiniTorrentDAL;
using MiniTorrentDAL.Messeges;
using MiniTorrentService.RequestData;
using Newtonsoft.Json;

namespace MiniTorrentService
{
    public class MiniTorrentService : IMiniTorrentService
    {
        private MiniTorrentCrud db = new MiniTorrentCrud();
        private JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Login a user. request should contain username, password, ip, port, files.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>username</returns>
        public string Login(LoginRequest req)
        {
            if (Authenticate(req.Username, req.Password))
            {
                string user = db.CreateLoggedInUser(req.Username, req.IP, req.Port, req.Files);
                if (user != null)
                {
                    return JsonConvert.SerializeObject(CreateUserJson(user));
                }
            }
            return JsonConvert.SerializeObject(CreateUserJson(""));
        }

        /// <summary>
        /// Logout a user. request should contain Username and Password fields.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>Username</returns>
        public string Logout(Request req)
        {
            if (Authenticate(req.Username, req.Password))
            {
                string user = db.DeleteLoggedInUser(req.Username);
                if (user != null)
                {
                    return serializer.Serialize(CreateUserJson(user));
                }
            }
            return serializer.Serialize(CreateUserJson(""));
        }

        /// <summary>
        /// Get all available files.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>file list</returns>
        public string GetFileList(Request req)
        {
            if (Authenticate(req.Username, req.Password))
            {
                List<FileObject> fileList = db.ReadFiles();
                if (fileList != null)
                {
                    return serializer.Serialize(CreateResponseJson(new { files = fileList }));
                }
            }

            return serializer.Serialize(CreateResponseJson(new {files = ""}));
        }

        /// <summary>
        /// NOT IMPLEMENTED.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string GetUserList(Request req)
        {
            if (Authenticate(req.Username, req.Password))
            {
                List<UserObject> userList = db.ReadUsers();
                if (userList != null)
                {
                    return serializer.Serialize(CreateResponseJson(new { users = userList }));
                }
            }

            return serializer.Serialize(CreateResponseJson(new { users = "" }));
        }

        /// <summary>
        /// Get user list by file name.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string GetUserListByFile(UserListPerFileRequest req)
        {
            if (Authenticate(req.Username, req.Password))
            {
                FileObject fileObject = db.ReadFile(req.FileName);
                if (fileObject != null)
                {
                    return serializer.Serialize(CreateResponseJson(new { file = fileObject }));
                }
            }

            return serializer.Serialize(CreateResponseJson(new { file = "" }));
        }

        /// <summary>
        /// Get file list by username.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string GetFileListByUser(FileListPerUserRequest req)
        {
            if (Authenticate(req.Username, req.Password))
            {
                LoggedInUserObject userObject = db.ReadLoggedInUser(req.TargetUsername);
                if (userObject != null)
                {
                    return serializer.Serialize(CreateResponseJson(new { user = userObject }));
                }
            }

            return serializer.Serialize(CreateResponseJson(new { user = "" }));
        }

        /// <summary>
        /// Updates the file list for a logged in user.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string UpdateFileListByUser(UpdateFilesRequest req)
        {
            if (Authenticate(req.Username, req.Password))
            {
                string username = db.UpdateFilesForLoggedInUser(req.Username, req.Files);
                return serializer.Serialize(CreateResponseJson(new { user = username }));
            }
            return serializer.Serialize(CreateResponseJson(new { user = "" }));
        }

        /// <summary>
        /// Authenticate the users credentials.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool Authenticate(string username, string password)
        {
            UserObject user = db.ReadUser(username);
            if (user != null && user.Password == password)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///  Create a user json string.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private object CreateUserJson(string user)
        {
            return CreateResponseJson(new {username = user });
        }

        /// <summary>
        /// create json response string.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private object CreateResponseJson(object json)
        {
            return new {response = json};
        }
    }
}
