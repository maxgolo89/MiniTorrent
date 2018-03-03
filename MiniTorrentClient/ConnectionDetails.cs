namespace MiniTorrent_GUI
{
    public class ConnectionDetails
    {
        public string ServerIpAddress { get; set; }
        public int IncomingTcpPort { get; set; }
        public int OutgoingTcpPort { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PublishedFilesSource { get; set; }
        public string DownloadedFilesDestination { get; set; }
    }
}
