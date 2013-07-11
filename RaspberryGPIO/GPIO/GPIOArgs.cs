using System;

namespace RaspberryGPIO.GPIO
{
    public class GPIOArgs : EventArgs
    {
        public bool Value { get; private set; }

        public GPIOArgs(bool value)
        {
            Value = value;
        }
    }
}
