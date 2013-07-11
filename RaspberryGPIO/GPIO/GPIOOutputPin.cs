
namespace RaspberryGPIO.GPIO
{
    public class GPIOOutputPin : GPIOPin
    {
        public GPIOOutputPin(PhysicalPin pin) : base(PinDirection.Out, pin)
        {

        }

        public new bool Read()
        {
            return base.Read();
        }

        public new void Write(bool value)
        {
            base.Write(value);
        }
    }
}
