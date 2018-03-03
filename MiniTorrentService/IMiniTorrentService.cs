using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using MiniTorrentService.RequestData;

namespace MiniTorrentService
{
    [ServiceContract]
    public interface IMiniTorrentService
    {
        /// <summary>
        /// Login user.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>Json string</returns>
        [OperationContract, WebInvoke(UriTemplate = "login", RequestFormat = WebMessageFormat.Json, Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped)]
        string Login(LoginRequest req);

        /// <summary>
        /// Logout user,
        /// </summary>
        /// <param name="req"></param>
        /// <returns>Json string</returns>
        [OperationContract, WebInvoke(UriTemplate = "logout", RequestFormat = WebMessageFormat.Json, Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped)]
        string Logout(Request req);

        /// <summary>
        /// Get currently available files.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>Json string</returns>
        [OperationContract, WebInvoke(UriTemplate = "filelist", RequestFormat = WebMessageFormat.Json, Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetFileList(Request req);

        /// <summary>
        /// Get currently logged in users.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>Json string</returns>
        [OperationContract, WebInvoke(UriTemplate = "userlist", RequestFormat = WebMessageFormat.Json, Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetUserList(Request req);

        /// <summary>
        /// Get currently available files by user.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>Json string</returns>
        [OperationContract, WebInvoke(UriTemplate = "filelistbyuser", RequestFormat = WebMessageFormat.Json, Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetFileListByUser(FileListPerUserRequest req);

        /// <summary>
        /// Get all owners of a file.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>Json string</returns>
        [OperationContract, WebInvoke(UriTemplate = "userlistbyfile", RequestFormat = WebMessageFormat.Json, Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetUserListByFile(UserListPerFileRequest req);

        /// <summary>
        /// Update the file list of a logged in user.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [OperationContract, WebInvoke(UriTemplate = "updatefilelistbyuser", RequestFormat = WebMessageFormat.Json, Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped)]
        string UpdateFileListByUser(UpdateFilesRequest req);
    }

}
