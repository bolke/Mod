using ModBase.Configuration.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBase.Interfaces
{
  public interface IExecutable : ILockable, IComparable, IConfigurable
  {
    [Configure(DefaultValue=true)]
    bool StartImmediately
    {
      get;
      set;
    }

    [Configure(DefaultValue = true)]
    bool IsEnabled
    {
      get;
      set;
    }

    [Configure(DefaultValue = 0)]
    uint ExecuteLimit
    {
      get;
      set;
    }

    [Configure(DefaultValue = 0)]
    uint ExecuteCount
    {
      get;
      set;
    }

    [Configure(DefaultValue = 0)]
    uint Interval
    {
      get;
      set;
    }

    [Configure(DefaultValue = 0)]
    uint ExecuteTick
    {
      get;
      set;
    }

    [Configure(DefaultValue = 0)]
    Int64 LateTicks
    {
      get;
      set;
    }

    [Configure(DefaultValue = 0)]
    uint LastTick
    {
      get;
      set;
    }

    [Configure(DefaultValue = 0)]
    uint Duration
    {
      get;
      set;
    }

    uint Tick
    {
      get;
    }

    bool IsFinished
    {
      get;
    }

    bool ReadyToRun
    {
      get;
    }

    bool Execute();
    bool ScheduleNext();
  }
}
