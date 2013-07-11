using FluentNHibernate.Mapping;
using SolarSystemDatabase.Models;

namespace SolarSystemDatabase.Mappings
{
    public class DeviceMap : ClassMap<Device>
    {
        public DeviceMap()
        {
            Table("Device");

            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.DevicePower);
            Map(x => x.IsRunning);
            Map(x => x.IsUserControlled);
            Map(x => x.Name);
            Map(x => x.Priority);
            Map(x => x.Pin);
            Map(x => x.RunTime);
            Map(x => x.StartTime);
        }
    }

    public class PowerMap : ClassMap<Power>
    {
        public PowerMap()
        {
            Table("Power");

            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Date);
            Map(x => x.PowerValue);
            Map(x => x.UsedPowerValue);
        }
    }

    public class LogMap : ClassMap<LogEntry>
    {
        public LogMap()
        {
            Table("Log");

            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Date);
            Map(x => x.Message).Length(4001);
            Map(x => x.Type);
        }
    }
}
