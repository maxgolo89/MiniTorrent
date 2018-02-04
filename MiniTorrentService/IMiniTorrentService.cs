using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentService
{
    [ServiceContract]
    public interface IMiniTorrentService
    {
        [OperationContract, WebInvoke(UriTemplate = "signIn", RequestFormat = WebMessageFormat.Json, Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped)]
        string SignIn(SignInData signInData);

        [OperationContract, WebInvoke(UriTemplate = "signOut", ResponseFormat = WebMessageFormat.Json, Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped)]
        string SignOut(SignOutData signOutData);

        [OperationContract, WebInvoke(UriTemplate = "request", ResponseFormat = WebMessageFormat.Json, Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped)]
        string Request(RequestData requestData);
    }

}
