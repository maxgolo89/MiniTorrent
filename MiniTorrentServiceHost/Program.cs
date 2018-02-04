using System;
using System.Collections.Generic;
using System.Linq;
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
            Uri httpUri = new Uri("http://localhost:8090/MiniTorrentService");
            WebServiceHost host = new WebServiceHost(typeof(MiniTorrentService.MiniTorrentService), httpUri);
            host.AddServiceEndpoint(typeof(IMiniTorrentService), new WebHttpBinding(), httpUri);
            ServiceDebugBehavior stp = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            stp.HttpHelpPageEnabled = false;
            
            host.Open();
            Console.WriteLine("Server is up...");
            Console.ReadKey();
        }
    }
}
