using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using SolarSystem.Log;

namespace SolarSystem.Utils
{
    class IPFinder
    {
        public const string IPStartPrefix = "192.168.1.";

        public delegate void IPFoundEvent(IPFoundEventArgs args);
        public event IPFoundEvent IPFound;

        private readonly Logger _logger;

        public IPFinder()
        {
            _logger = Logger.Instance.Value;
        }

        public void FindIP()
        {
            IPFound(new IPFoundEventArgs("192.168.1.168"));
            //var addresses = Dns.GetHostAddresses(Dns.GetHostName()).Where(x => x.AddressFamily == AddressFamily.InterNetwork).ToList();
            //if (addresses.Count == 0) {
            //    _logger.Log("There are no IPs on your machine!", LogType.ERROR);
            //    return;
            //}

            //var foundAddress = addresses.Any(ipAddress => ipAddress.ToString().StartsWith(IPStartPrefix));

            //if (foundAddress == false) {
            //    _logger.Log("Found no IPs matching the IP prefix!", LogType.ERROR);
            //    return;
            //}

            //FindWebboxIP();
        }

        private void FindWebboxIP()
        {
            for (var i = 1; i < 255; i++)
            {
                var IP = "http://" + IPStartPrefix + i;

                var client = new WebClient();
                client.DownloadStringCompleted += client_DownloadStringCompleted;
                client.DownloadStringAsync(new Uri(IP), IP);
            }
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try {
                var str = e.Result;
                if (str.ToLower().Contains("sunny webbox")) {
                    _logger.Log("Webbox found at " + (string)e.UserState);
                    IPFound(new IPFoundEventArgs((string)e.UserState));
                }
            } catch (Exception) { }
        }
    }
}
