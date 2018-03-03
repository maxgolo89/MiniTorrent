using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniTorrent_MediationServerContract;
using System.IO;

namespace MiniTorrent_GUI
{
    class FilesHelper
    {
        public const long ONE_KB = 1024;
        public const long ONE_MB = ONE_KB * 1024;

        public static List<FileDetails> getFilesList(string source)
        {
            List<FileDetails> filesList = new List<FileDetails>();

            if (!Directory.Exists(source))
            {
                //No files to publish returning empty list.
                return filesList;
            }

            string[] filesPaths = Directory.GetFiles(source);
            foreach (string file in filesPaths)
            {
                FileDetails curr = getFileDetailes(file);
                if (curr != null)
                    filesList.Add(curr);
            }

            return filesList;
        }

        public static FileDetails getFileDetailes(string path)
        {
            FileDetails file = new FileDetails();
            if (!File.Exists(path))
                return null;

            long sizeInBytes = new FileInfo(path).Length;
            file.Size = ((float)sizeInBytes) / ONE_MB;
            file.Name = System.IO.Path.GetFileName(path);
            file.Count = -1;

            return file;
        }
    }
}
