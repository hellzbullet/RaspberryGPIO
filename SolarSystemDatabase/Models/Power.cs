
namespace SolarSystemDatabase.Models
{
    public class Power
    {
        public virtual int Id { get; set; }
        public virtual long Date { get; set; }
        public virtual int PowerValue { get; set; }
        public virtual int UsedPowerValue { get; set; }
    }
}
