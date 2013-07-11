using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Utils
{
    class PowerChangedEventArgs : EventArgs
    {
        public int Power { get; private set; }

        public PowerChangedEventArgs(int power)
        {
            Power = power;
        }
    }
}
