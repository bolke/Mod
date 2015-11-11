using Mod.Configuration.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Interfaces
{
  public interface IRunnable : ILockable
  {
    Delegate Delegated
    {
      get;
      set;
    }

    object[] DelegateParameters
    {
      get;
      set;
    }

    [Configure(DefaultValue = false)]
    bool IsStarted
    {
      get;
      set;
    }

    [Configure(DefaultValue = false)]
    bool IsStopped
    {
      get;
      set;
    }

    [Configure(DefaultValue = false)]
    bool IsRunning
    {
      get;
      set;
    }

    [Configure(DefaultValue = false)]
    bool IsPaused
    {
      get;
      set;
    }

    [Configure(DefaultValue = false)]
    bool AutoStart
    {
      get;
      set;
    }

    bool Start();

    bool Pause();

    bool Resume();

    bool Stop();
  }
}
