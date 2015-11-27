using Mod.Configuration.Modules;
using Mod.Configuration.Properties;
using Mod.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Modules.Abstracts
{
  public abstract class Executable : Lockable, IExecutable
  {
    private bool enabled = true;
    private uint tock = 0;
    private uint tick = 0;
    private uint lastTick = 0;
    private Int64 lateTicks = 0;
    private uint executeTick = 0;
    private uint duration = 0;
    private uint executeCount = 0;
    private uint executeLimit = 0;
    private uint interval = 0;
    private bool startImmediately = false;

    [Configure(DefaultValue = 0)]
    public virtual uint ExecuteTick
    {
      get { lock (Padlock) return executeTick; }
      set { lock (Padlock) executeTick = value; }
    }

    [Configure(DefaultValue = 0)]
    public virtual uint LastTick
    {
      get { lock (Padlock) return lastTick; }
      set { lock (Padlock) lastTick = value; }
    }

    [Configure(DefaultValue = 0)]
    public virtual uint Duration
    {
      get { lock (Padlock) return duration; }
      set { lock (Padlock) duration = value; }
    }

    [Configure(DefaultValue = 0)]
    public virtual uint ExecuteCount
    {
      get
      {
        lock (Padlock) return executeCount;
      }
      set
      {
        lock (Padlock) executeCount = value;
      }
    }

    [Configure(DefaultValue = 0)]
    public virtual uint ExecuteLimit
    {
      get
      {
        lock (Padlock) return executeLimit;
      }
      set
      {
        lock (Padlock) executeLimit = value;
      }
    }

    [Configure(DefaultValue = true)]
    public virtual bool IsEnabled
    {
      get
      {
        lock (Padlock) return enabled;
      }
      set
      {
        lock (Padlock) enabled = value;
      }
    }

    [Configure(DefaultValue = 0)]
    public virtual uint Interval
    {
      get
      {
        lock (Padlock) return interval;
      }
      set
      {
        lock (Padlock) interval = value;
      }
    }

    [Configure(DefaultValue = 0)]
    public virtual long LateTicks
    {
      get
      {
        lock (Padlock) return lateTicks;
      }
      set
      {
        lock (Padlock) LateTicks = value;
      }
    }

    [Configure(DefaultValue = false)]
    public virtual bool StartImmediately
    {
      get { lock (Padlock) return startImmediately; }
      set { lock (Padlock) startImmediately = value; }
    }
    
    public virtual uint Tick
    {
      get
      {
        lock (Padlock)
        {
          tock = tick;
          int result = Environment.TickCount;
          result++;
          try
          {
            if (result < 0)
              tick = Convert.ToUInt32(-result) + 0x80000000;
            else
              tick = Convert.ToUInt32(result);
          }
          catch
          {
            tick = 0;
          }
          if (tick < tock)
            ClockOverflowControl();
          return tick;
        }
      }
    }

    public virtual bool IsFinished
    {
      get { lock (Padlock) return ((executeLimit > 0) && (executeLimit <= executeCount)); }
    }

    public virtual bool ReadyToRun
    {
      get { lock (Padlock) return IsEnabled && !IsFinished && ExecuteTick <= Tick; }
    }

    protected void ClockOverflowControl()
    {
      if (executeTick > tock)
        executeTick = (executeTick - tock) + tick;
      else
        executeTick = tick;
    }

    public virtual bool Execute()
    {
      lock (Padlock)
      {
        if (ReadyToRun)
        {
          LastTick = Tick;
          ExecuteCount++;
          ScheduleNext();
          return true;
        }
        return false;
      }
    }

    public override bool Initialize()
    {
      if(base.Initialize())
      {
        ScheduleNext();
        return true;
      }
      return false;
    }

    public virtual int CompareTo(object other)
    {
      int result = -1;
      lock (Padlock)
      {
        if (other != null)
        {
          Executable otherExecutable = other as Executable;
          if (otherExecutable != null)
          {
            if (!this.ReadyToRun)
            {
              if (otherExecutable.ReadyToRun)
                return 1;
              return 0;
            }
            else if (otherExecutable.ReadyToRun)
            {
              if (otherExecutable.ExecuteTick < ExecuteTick)
                return 1;
            }
          }
        }
        return result;
      }
    }

    public virtual bool ScheduleNext()
    {
      lock (Padlock)
      {
        if (!IsFinished)
        {
          if (StartImmediately && ExecuteCount == 0)
            ExecuteTick = Tick;
          else
            ExecuteTick = Tick + Interval;
          return true;
        }
      }
      return false;
    }
  }
}
