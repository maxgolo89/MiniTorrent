using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using MiniTorrentService;


namespace MiniTorrentServiceHost
{
    class Program
    {
        public static void Main(string[] args)
        {
            string ownEndPointAddress = HostIpAddressHelper();
            Uri httpUri = new Uri("http://" + ownEndPointAddress + ":8090/MiniTorrentService");
            WebServiceHost host = new WebServiceHost(typeof(MiniTorrentService.MiniTorrentService), httpUri);
            host.AddServiceEndpoint(typeof(IMiniTorrentService), new WebHttpBinding(), httpUri);
            ServiceDebugBehavior stp = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            stp.HttpHelpPageEnabled = false;
            host.Open();
            Console.WriteLine("Server is up and running on " + ownEndPointAddress + ":8090");
            Console.ReadKey();
        }

        private static string HostIpAddressHelper()
        {
            var host = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var ip in host)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "localhost";
        }
    }
}
