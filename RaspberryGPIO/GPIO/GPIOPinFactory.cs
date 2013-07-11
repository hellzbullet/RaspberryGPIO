using System;
using System.Collections.Generic;

namespace RaspberryGPIO.GPIO
{
    public class GPIOPinFactory : IDisposable
    {
        public static Dictionary<uint, GPIOPin> UsedPins = new Dictionary<uint, GPIOPin>();

        public static Lazy<GPIOPinFactory> Instance = new Lazy<GPIOPinFactory>(); 

        private bool _disposed;

        ~GPIOPinFactory()
        {
            if (!_disposed) Dispose();
        }

        public GPIOPin CreatePin(PinDirection direction, PhysicalPin pin)
        {
            if (UsedPins.ContainsKey((uint)pin)) {
                if (direction == UsedPins[(uint) pin].Direction) return UsedPins[(uint) pin];
                throw new Exception("Pin already in use. Use RemapPin() to remap the pin.");
            }

            GPIOPin returnPin;

            if (direction == PinDirection.In) {
                returnPin = new GPIOInputPin(pin);
            } else {
                returnPin = new GPIOOutputPin(pin);
            }

            UsedPins.Add((uint)returnPin.Pin, returnPin);

            return returnPin;
        }

        public GPIOInputPin CreateInputPin(PhysicalPin pin)
        {
            return (GPIOInputPin) CreatePin(PinDirection.In, pin);
        }

        public GPIOOutputPin CreateOutputPin(PhysicalPin pin)
        {
            return (GPIOOutputPin)CreatePin(PinDirection.Out, pin);
        }

        public GPIOPin RemapPin(PinDirection direction, PhysicalPin pin)
        {
            if (UsedPins.ContainsKey((uint)pin)) {
                if (direction == UsedPins[(uint) pin].Direction) return UsedPins[(uint) pin];

                UsedPins[(uint)pin].Dispose();
                UsedPins.Remove((uint) pin);
            }

            return CreatePin(direction, pin);
        }

        public GPIOPin RemapPin(PinDirection direction, GPIOPin pin)
        {
            if (direction == pin.Direction) return pin;

            return RemapPin(direction, pin.Pin);
        }

        public void Dispose()
        {
            _disposed = true;
            GC.SuppressFinalize(this);
            foreach (var key in UsedPins.Keys)
            {
                UsedPins[key].Dispose();
            }
        }
    }
}
