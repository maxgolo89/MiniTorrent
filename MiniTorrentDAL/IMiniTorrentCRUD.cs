using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using MiniTorrentDAL.Messeges;

namespace MiniTorrentDAL
{
    interface IMiniTorrentCrud
    {
        #region Create Operations
        /// <summary>
        /// Insert new User entry to database.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>username, or null on failure.</returns>
        string CreateUser(string username, string password);

        /// <summary>
        /// Insert new LoggedInUser entry to database.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="files"></param>
        /// <returns>username, or null on failure.</returns>
        string CreateLoggedInUser(string username, string ip, int port, Dictionary<string, int> files);

        #endregion

        #region Update Operations
        /// <summary>
        /// Update LoggedInUser file list.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="files"></param>
        /// <returns>username, or null on failure.</returns>
        string UpdateFilesForLoggedInUser(string username, Dictionary<string, int> files);

        /// <summary>
        /// Update LoggedInUser time stamp.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>username, or null on failure.</returns>
        string UpdateLoggedInUserTimeStamp(string username);
        #endregion

        #region Read Operations
        /// <summary>
        /// Retrieve all users from database.
        /// </summary>
        /// <returns>List of UserObject.</returns>
        List<UserObject> ReadUsers();

        /// <summary>
        /// Retireve specific user from database.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>UserObject, or null on failure.</returns>
        UserObject ReadUser(string username);

        /// <summary>
        /// Retrieve all files from database.
        /// </summary>
        /// <returns>List of FileObjects.</returns>
        List<FileObject> ReadFiles();

        /// <summary>
        /// Retrieve specific file from database.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>FileObject, or null on failure.</returns>
        FileObject ReadFile(string name);

        /// <summary>
        /// Retrieve all loggedinusers from database.
        /// </summary>
        /// <returns>List of LoggedInUserObject's.</returns>
        List<LoggedInUserObject> ReadLoggedInUsers();

        /// <summary>
        /// Retrieve specific loggedinuser from database.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>LoggedInUserObject, or null on failure.</returns>
        LoggedInUserObject ReadLoggedInUser(string username);

        #endregion

        #region Delete Operations
        /// <summary>
        /// Delete specific user from database.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>username, or null on failure.</returns>
        string DeleteUser(string username);

        /// <summary>
        /// Delete all users in database.
        /// *** USE WITH CAUTION! ***
        /// </summary>
        /// <returns>true - success, false - failure.</returns>
        bool DeleteUsers();

        /// <summary>
        /// Delete specific loggedinuser from database.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>username, or null on failure.</returns>
        string DeleteLoggedInUser(string username);

        /// <summary>
        /// Delete all loggedinusers from database.
        /// *** USE WITH CAUTION! ***
        /// </summary>
        /// <returns>true - success, false - failure.</returns>
        bool DeleteLoggedInUsers();

        #endregion
    }
}
