using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentDBService
{
    [ServiceContract]    
    interface IMiniTorrentDBService
    {
            /// <summary>
            /// Register new users. Json param format:
            /// { userCredentials: 
            ///     { username: username, 
            ///       password: password 
            ///     }
            /// }
            /// </summary>
            /// <param name="userCredentials"></param>
            /// <returns>
            /// { success: true/false }
            /// </returns>
            [OperationContract,
             WebInvoke(
                UriTemplate = "register",
                RequestFormat = WebMessageFormat.Json,
                Method = "POST",
                BodyStyle = WebMessageBodyStyle.Wrapped)]
            string Register(UserCredentials userCredentials);

        /// <summary>
        /// Login a user. Json param format:
        /// { userCredential: 
        ///     { username: username,
        ///       password: password 
        ///     }
        ///   fileList: [filename_1, filename_2, ...]
        /// }
        /// </summary>
        /// <param name="userCredentials"></param>
        /// <param name="fileList"></param>
        /// <returns>
        /// { sessionId: sessionId }
        /// </returns>
        [OperationContract,
         WebInvoke(
             UriTemplate = "login",
             RequestFormat = WebMessageFormat.Json,
             Method = "POST",
             BodyStyle = WebMessageBodyStyle.Wrapped)]
        string Login(UserCredentials userCredentials, List<string> fileList);

        /// <summary>
        /// Logout a user. Json param format:
        /// { sessionId: sessionId }
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns>
        /// { success: true/false }
        /// </returns>
        [OperationContract,
         WebInvoke(
             UriTemplate = "logout",
             RequestFormat = WebMessageFormat.Json,
             Method = "POST",
             BodyStyle = WebMessageBodyStyle.Wrapped)]
        string Logout(string sessionId);

        /// <summary>
        /// Get a list of all currently available files. Json param format:
        /// { sessionId: sessionId }
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns>
        /// { fileList: [filename_1, filename_2, ...] }
        /// </returns>
        [OperationContract,
         WebInvoke(
             UriTemplate = "getAllFiles",
             RequestFormat = WebMessageFormat.Json,
             Method = "POST",
             BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetAllFiles(string sessionId);

        /// <summary>
        /// Get specific file. Json param format:
        /// { sessionId: sessionId
        ///   fileName: fileName 
        /// }
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="fileName"></param>
        /// <returns>
        /// { results: 
        ///     [{username: username, address: address, port: port}, ...] 
        /// }
        /// </returns>
        [OperationContract,
         WebInvoke(
             UriTemplate = "getFileByName",
             RequestFormat = WebMessageFormat.Json,
             Method = "POST",
             BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetFileByName(string sessionId, string fileName);

        /// <summary>
        /// Keep alive signal from user client, in order to keep session alive, and keep file list updated. Json param format:
        /// { sessionId: sessionId }
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns>
        /// { success: true/false }
        /// </returns>
        [OperationContract,
         WebInvoke(
             UriTemplate = "keepalive",
             RequestFormat = WebMessageFormat.Json,
             Method = "POST",
             BodyStyle = WebMessageBodyStyle.Wrapped)]
        string KeepAlive(string sessionId);
    }
}
