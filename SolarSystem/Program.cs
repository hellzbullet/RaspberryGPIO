using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RaspberryGPIO.GPIO;
using SolarSystem.Log;
using SolarSystem.Utils;
using SolarSystem.System;

namespace SolarSystem
{
    public class Program
    {
        public static AutoResetEvent AutoReset = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            new System.SolarSystem().Run();

            GPIOPinFactory.Instance.Value.Dispose();
        }
    }
}
