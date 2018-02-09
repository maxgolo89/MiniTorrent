using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentService
{
    public class MiniTorrentService : IMiniTorrentService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string SignIn(SignInData request)
        {
            Console.WriteLine("In SignIn");
            Console.WriteLine(request.ToString());
            return "Signed In";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string SignOut(SignOutData request)
        {
            Console.WriteLine("In SignOut");
            Console.WriteLine(request.ToString());
            return "Signed Out";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string Request(RequestData request)
        {
            Console.WriteLine("In Request");
            Console.WriteLine(request.ToString());
            return "Requested Something";
        }
    }
}
