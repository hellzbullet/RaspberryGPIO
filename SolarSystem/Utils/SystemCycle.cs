using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SolarSystem.Log;

namespace SolarSystem.Utils
{
    public class SystemCycle
    {
        private static readonly Mutex Mutex = new Mutex();

        public static void DoCycle()
        {
            if (!Mutex.WaitOne(3000)) return;

            var reader = new PowerReader();
            reader.PowerChanged += reader_PowerChanged;
            reader.ReadingCancelled += reader_ReadingCancelled;
            reader.ReadPower();
        }

        private static void reader_ReadingCancelled(object sender, EventArgs e)
        {
            Mutex.ReleaseMutex();
        }

        private static void reader_PowerChanged(PowerChangedEventArgs args)
        {
            PowerOutput.Output(args.Power);

            Mutex.ReleaseMutex();
        }
    }
}
