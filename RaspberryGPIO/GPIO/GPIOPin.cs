using System;
using System.IO;

namespace RaspberryGPIO.GPIO
{
    public class GPIOPin : IDisposable
    {
        public delegate void EventHandler(GPIOPin pin, GPIOArgs args);

        public PhysicalPin Pin { get; private set; }
        public PinDirection Direction { get; private set; }

        private readonly object Lock = new object();

        protected GPIOPin(PinDirection direction, PhysicalPin pin)
        {
            Pin = pin;
            Direction = direction;

            ExportPin();
        }

        private void ExportPin()
        {
            lock (Lock) {
                if (!Directory.Exists(GPIOConstants.FilePath + "gpio" + (uint)Pin))
                {
                    Directory.CreateDirectory(GPIOConstants.FilePath + "gpio" + (uint)Pin);
                    if (!File.Exists(GPIOConstants.FilePath + "export")) File.Create(GPIOConstants.FilePath + "export");
                    File.WriteAllText(GPIOConstants.FilePath + "export", ((uint)Pin).ToString());
                }

                if (!File.Exists(GPIOConstants.FilePath + "gpio" + (uint)Pin + "/direction"))
                    File.Create(GPIOConstants.FilePath + "gpio" + (uint)Pin + "/direction");
                File.WriteAllText(GPIOConstants.FilePath + "gpio" + (uint)Pin + "/direction", Direction.ToString().ToLower());
            }
        }

        protected virtual bool Read()
        {
            lock (Lock) {
                var readValue = File.ReadAllText(GPIOConstants.FilePath + "gpio" + (uint)Pin + "/value");
                return (readValue.Length > 0 && readValue[0] == '1');
            }
        }

        protected virtual void Write(bool value)
        {
            lock (Lock) {
                if (!File.Exists(GPIOConstants.FilePath + "gpio" + (uint)Pin + "/direction"))
                    File.Create(GPIOConstants.FilePath + "gpio" + (uint)Pin + "/direction");
                File.WriteAllText(GPIOConstants.FilePath + "gpio" + (uint)Pin + "/value", value ? "1" : "0");
            }
        }

        public void Dispose()
        {
            lock (Lock) {
                File.WriteAllText(GPIOConstants.FilePath + "unexport", ((uint)Pin).ToString());
            }
        }
    }
}
