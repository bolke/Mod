using ModBase.Configuration.Properties;
using ModBase.Interfaces;
using ModBase.Interfaces.Pipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBase.Modules
{
  [Configure(InitType = typeof(Worker<IJob>))]
  public class Worker<T> : Job, IWorker<T> where T : IJob
  {
    private IPipe<T> pool = null;
    private IPipe<T> finished = null;

    [Configure(InitType = typeof(Pipe<>))]
    public virtual IPipe<T> Pool
    {
      get
      {
        lock (Padlock) return pool;
      }
      set
      {
        lock (Padlock) pool = value;
      }
    }

    [Configure(InitType = typeof(Pipe<>))]
    public virtual IPipe<T> Finished
    {
      get
      {
        lock (Padlock) return finished;
      }
      set
      {
        lock (Padlock) finished = value;
      }
    }

    public override bool Initialize()
    {
      if (base.Initialize())
      {        
        Pool.Initialize();
        Finished.Initialize();
        return true;
      }
      return false;
    }

    public override bool Work()
    {
      bool result = false;
      T job;
      lock (Padlock) job = pool.Pop();
      if (job != null)
      {
        if (job.ReadyToRun)
        {          
          result = job.Execute();
          if (!result)
            job.CrashCount++;
        }
        if (job.IsFinished)
          if (job.SaveOnFinish)
            lock (Padlock) finished.Push(job);
          else
            job.Cleanup();
        else
        {
          lock (Padlock) pool.Push(job);
        }
      }      
      return result;
    }

    public override void Dispose()
    {
      base.Dispose();
    }
  }
}
