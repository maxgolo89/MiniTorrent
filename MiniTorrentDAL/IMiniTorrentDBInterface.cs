using System;
using System.Collections.Generic;
using System.Data;

namespace MiniTorrentDAL
{
    public interface IMiniTorrentDBInterface
    {
        /// <summary>
        /// Insert new user to user table.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        /// true - success.
        /// false - failed.
        /// </returns>
        bool InsertNewUser(User user);

        /// <summary>
        /// Insert existing user to loggedinuser table.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="sessionId"></param>
        /// <param name="timeStamp"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="files"></param>
        /// <returns>
        /// true - success.
        /// false - failed.
        /// </returns>
        bool InsertUserToLoggedInUser(User user, string sessionId, DateTime timeStamp, string ip, int port, List<FileInformation> files);

        /// <summary>
        /// Remove user from loggedinuser table.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns>
        /// true - success.
        /// false - failed.
        /// </returns>
        bool RemoveUserFromLoggedInUser(string sessionId);

        /// <summary>
        /// Remove user from loggedinuser table.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        /// true - success.
        /// false - failed.
        /// </returns>
        bool RemoveUserFromLoggedInUser(User user);

        /// <summary>
        /// Get specific file inforamtion.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>
        /// FileInformation object, contains name, size, and resources.
        /// </returns>
        FileInformation GetFile(string name);

        /// <summary>
        /// Get all file informations availble.
        /// </summary>
        /// <returns>
        /// List<FileInformation> containing file names, sizes, and resources.
        /// </returns>
        List<FileInformation> GetAllFiles();

        /// <summary>
        /// Updates currently availble files in fileinformation, loggedinuser, loggedinuserfileinformations tables.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="files"></param>
        /// <returns>
        /// true - success.
        /// false - failed.
        /// </returns>
        bool UpdateFileList(string sessionId, List<FileInformation> files);

        /// <summary>
        /// Updates currently availble files in fileinformation, loggedinuser, loggedinuserfileinformations tables.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="files"></param>
        /// <returns>
        /// true - success.
        /// false - failed.
        /// </returns>
        bool UpdateFileList(User user, List<FileInformation> files);

        /// <summary>
        /// Update time stamp on loggedinuser.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="timeStamp"></param>
        /// <returns>
        /// true - success.
        /// false - failed.
        /// </returns>
        bool UpdateTimeStamp(string sessionId, DateTime timeStamp);
    }
}