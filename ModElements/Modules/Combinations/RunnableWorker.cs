using Mod.Configuration.Properties;
using Mod.Interfaces;
using Mod.Modules.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Modules.Combinations
{
  public class RunnableWorker<T>: Lockable
  {
    private delegate bool DoWork();
    private IWorker<T> worker = null;
    private IRunnable runnable = null;

    [Configure(InitType = typeof(Worker<>))]
    public IWorker<T> Worker
    {
      get { lock(Padlock) return worker; }
      set
      {
        lock(Padlock)
        {
          worker = value;
          if(Runnable != null)
            Runnable.Delegated = new DoWork(Worker.Execute);
        }
      }
    }

    [Configure(IsRequired=true)]
    public IRunnable Runnable
    {
      get { lock(Padlock) return runnable; }
      set
      {
        lock(Padlock)
        {
          runnable = value;
          if(Worker != null)
            Runnable.Delegated = new DoWork(Worker.Execute);
        }
      }
    }

    public override bool Initialize()
    {
      if(base.Initialize())
      {
        Runnable.Delegated = new DoWork(Worker.Execute);
        return true;
      }
      return false;
    }

  }
}
