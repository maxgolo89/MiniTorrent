using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentDBService
{
    public class MiniTorrentDBService : IMiniTorrentDBService
    {
        public string Register(UserCredentials userCredentials)
        {
            throw new NotImplementedException();
        }

        public string Login(UserCredentials userCredentials, List<string> fileList)
        {
            throw new NotImplementedException();
        }

        public string Logout(string sessionId)
        {
            throw new NotImplementedException();
        }

        public string GetAllFiles(string sessionId)
        {
            throw new NotImplementedException();
        }

        public string GetFileByName(string sessionId, string fileName)
        {
            throw new NotImplementedException();
        }

        public string KeepAlive(string sessionId)
        {
            throw new NotImplementedException();
        }
    }
}
