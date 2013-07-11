using System;

namespace SolarSystemDatabase.Models
{
    public class Device
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int DevicePower { get; set; }
        public virtual int Pin { get; set; }
        public virtual long? StartTime { get; set; }
        public virtual bool IsRunning { get; set; }
        public virtual bool IsUserControlled { get; set; }
        public virtual int Priority { get; set; }
        public virtual long RunTime { get; set; }
        
        public virtual void Start(bool userStarted)
        {
            IsRunning = true;
            IsUserControlled = userStarted;
            StartTime = DateTime.Now.Ticks;
        }

        public virtual void Stop(bool userStopped)
        {
            if (IsRunning == false) return;

            IsRunning = false;
            IsUserControlled = userStopped;
            RunTime += (long)(DateTime.Now.Ticks - StartTime);
            StartTime = null;
        }
    }
}
