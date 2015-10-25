using ModBase.Configuration.Properties;
using ModBase.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModElements.Modules {
  public class ThreadRunnable: Runnable {
    private Thread thread=null;
    private int runDelay = 1;

    [Configure(DefaultValue=1)]
    public int RunDelay
    {
      get { lock(Padlock) return runDelay; }
      set { lock(Padlock) runDelay = value; }
    }

    public override bool Start() {
      if(base.Start())
      {
        thread.Start();
        return true;
      }
      return false;
    }
    
    public override bool Stop() {
      return base.Stop();
    }

    public override bool Resume() {
      return base.Resume();
    }
    
    public override bool Pause() {
      return base.Pause();
    }

    protected void ThreadMain() {
      while(IsRunning){
        if(!IsPaused){
          Delegated.DynamicInvoke(DelegateParameters);
        }
        Thread.Sleep(RunDelay);
      }
    }

    public override bool Initialize()
    {
      if(base.Initialize())
      {
        thread = new Thread(this.ThreadMain);
        return true;
      }
      return false;
    }
  }
}
