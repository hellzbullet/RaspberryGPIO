using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Utils
{
    public class IPFoundEventArgs : EventArgs
    {
        public string IP { get; private set; }

        public IPFoundEventArgs(string ip)
        {
            IP = ip;
        }
    }
}
