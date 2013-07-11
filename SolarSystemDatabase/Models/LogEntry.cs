
namespace SolarSystemDatabase.Models
{
    public class LogEntry
    {
        public virtual int Id { get; set; }
        public virtual long Date { get; set; }
        public virtual string Message { get; set; }
        public virtual string Type { get; set; }
    }
}
