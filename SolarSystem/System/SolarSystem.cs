using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SolarSystem.Log;
using SolarSystem.Utils;
using SolarSystemDatabase.Models;

namespace SolarSystem.System
{
    public class SolarSystem
    {
        private volatile bool _programRunning = true;

        public void Run()
        {
            Logger.Instance.Value.Log("Starting program!");

            while (_programRunning) {
                SystemCycle.DoCycle();
                lock (this) {
                    Monitor.Wait(this, 10000);
                }
            }

            Logger.Instance.Value.Log("Shutting down program!");

            Program.AutoReset.Set();
        }
    }
}
