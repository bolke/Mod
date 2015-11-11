using Mod.Configuration.Properties;
using Mod.Interfaces;
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
    private IBasePipe basePipe = null;    

    [Configure(InitType=typeof(BasePipe))]
    public virtual IBasePipe BasePipe
    {
      get { return basePipe; }
      set
      {
        lock (Padlock) basePipe = value;
      }
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
      return (T)basePipe.PopObject();     
    }

    public virtual bool Push(T element)
    {
      return basePipe.PushObject(element);      
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
