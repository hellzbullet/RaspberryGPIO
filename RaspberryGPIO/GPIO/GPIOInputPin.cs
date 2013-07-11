
namespace RaspberryGPIO.GPIO
{
    public class GPIOInputPin : GPIOPin
    {
        public GPIOInputPin(PhysicalPin pin) : base(PinDirection.In, pin)
        {
            
        }
    }
}
