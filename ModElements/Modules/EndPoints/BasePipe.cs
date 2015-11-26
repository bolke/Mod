using Mod.Interfaces;
using Mod.Modules.Abstracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Modules.EndPoints
{
  public class BasePipe: IObjectContainer
  {
    private ConcurrentQueue<object> queue = new ConcurrentQueue<Object>();
    
    public virtual object PopObject()
    {
      object result = null;
      queue.TryDequeue(out result);
      return result;
    }

    public virtual bool PushObject(object element)
    {
      queue.Enqueue(element);
      return true;
    }
  }
}
