using Mod.Configuration.Properties;
using Mod.Interfaces.Pipes;
using Mod.Modules.Abstracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Modules.EndPoints
{
  public class QueuePipe<T> : Lockable, IQueuePipe<T>
  {
    private ConcurrentQueue<T> queue = null;

    [Configure(InitType=typeof(ConcurrentQueue<>))]
    public virtual ConcurrentQueue<T> Queue{
      get{ return queue;}
      set{ lock(Padlock) queue = value; }    
    }

    public override bool Initialize()
    {
      return base.Initialize();
    }

    public override bool Cleanup()
    {
      return base.Cleanup();
    }

    public virtual T Pop()
    {
      T result = default(T);
      queue.TryDequeue(out result);
      return result;
    }

    public bool Push(T element)
    {
      queue.Enqueue(element);
      return true;
    }

    public virtual Configuration.Modules.ModuleConfig ToConfig()
    {
      throw new NotImplementedException();
    }

    public virtual bool FromConfig(Configuration.Modules.ModuleConfig config)
    {
      throw new NotImplementedException();
    }

    public virtual object PopObject()
    {
      return Pop();
    }

    public virtual bool PushObject(object element)
    {
      return Push((T)element);
    }
  }
}
