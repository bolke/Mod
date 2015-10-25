using ModBase.Configuration.Properties;
using ModBase.Interfaces;
using ModBase.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBase.Combinations {
  public class RunnableWorker<T>: Lockable {
    private delegate bool DoWork();

    [Configure(InitType=typeof(Worker<>))]
    public IWorker<T> Worker {
      get;
      set;
    }

    [Configure(InitType=typeof(Runnable))]
    public IRunnable Runnable {
      get;
      set;
    }

    public override bool Initialize() {
      if(base.Initialize()) {
        Runnable.Delegated=new DoWork(Worker.Execute);
        return true;
      }
      return false;
    }

  }
}
