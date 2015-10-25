using ModBase.Configuration.Properties;
using ModBase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBase.Modules
{
  public class Runnable: Lockable, IRunnable
  {
    #region Variables

    private bool stopped = false;
    private bool started = false;
    private bool running = false;
    private bool paused = false;
    private bool autoStart = false;

    private object[] delegateParameters = null;
    private Delegate delegated = null;

    #endregion Variables

    [Configure(DefaultValue = false)]
    public virtual bool AutoStart
    {
      get
      {
        lock(Padlock) return autoStart;
      }
      set
      {
        lock(Padlock)
          autoStart = value;
      }
    }

    public virtual Delegate Delegated
    {
      get
      {
        lock(Padlock) return delegated;
      }
      set
      {
        if(!IsRunning)
        {
          lock(Padlock)
          {
            if(!IsRunning)
            {
              delegated = value;
            }
          }
        }
      }
    }

    public virtual object[] DelegateParameters
    {
      get
      {
        lock(Padlock) return delegateParameters;
      }
      set
      {
        lock(Padlock) delegateParameters = value;
      }
    }

    public virtual bool IsStarted
    {
      get
      {
        lock(Padlock) return started;
      }
      set
      {
        lock(Padlock)
          started = value;
      }
    }

    public virtual bool IsStopped
    {
      get
      {
        lock(Padlock) return stopped;
      }
      set
      {
        lock(Padlock)
          stopped = value;
      }
    }

    public virtual bool IsRunning
    {
      get
      {
        lock(Padlock) return (running);
      }
      set
      {
        lock(Padlock)
          running = value;
      }
    }

    public virtual bool IsPaused
    {
      get
      {
        lock(Padlock) return paused;
      }
      set
      {
        lock(Padlock)
          paused = value;
      }
    }

    public virtual bool Start()
    {
      lock(Padlock)
      {
        if(!IsStarted)
        {
          IsRunning = true;
          IsStarted = true;
          return true;
        }
      }
      return false;
    }

    public virtual bool Pause()
    {
      lock(Padlock)
      {
        if((IsStarted) && (!IsPaused))
        {
          IsPaused = true;
          return true;
        }
      }
      return false;
    }

    public virtual bool Resume()
    {
      lock(Padlock)
      {
        if(IsPaused)
        {
          IsPaused = false;
          return true;
        }
      }
      return false;
    }

    public virtual bool Stop()
    {
      lock(Padlock)
      {
        if(!IsStopped)
        {
          IsStopped = true;
          IsStarted = false;
          IsRunning = false;
          return true;
        }
      }
      return false;
    }

    public override bool Initialize()
    {
      if(base.Initialize())
      {
        IsStarted = false;
        IsStopped = false;
        IsPaused = false;
        IsRunning = false;
        return true;
      }
      return false;
    }

    public override bool Cleanup()
    {
      if(base.Cleanup())
      {
        IsStarted = false;
        IsStopped = false;
        IsPaused = false;
        IsRunning = false;
        return true;
      }
      return false;
    }

  }
}
