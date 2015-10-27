using Mod.Configuration.Modules;
using Mod.Configuration.Properties;
using Mod.Interfaces.Pipes;
using Mod.Modules;
using Mod.Modules.Abstracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Modules.Combinations
{
  public abstract class StreamPipe<T> : Lockable, IQueuePipe<T>
  {
    private byte[] incomingBuffer = new byte[1024];
    private Stream stream = null;
    private Pipe<T> pipe = null;    

    public Stream Stream
    {
      get { lock (Padlock) return stream; }
      set { 
        lock (Padlock) 
        { 
          stream = value; 
          if(stream!=null){
            if (stream.CanRead)
              stream.BeginRead(incomingBuffer, 0, 1024, new AsyncCallback(this.AsyncReadReady),new object());
          }
        } 
      }
    }

    [Configure(InitType = typeof(Pipe<>))]
    public Pipe<T> Pipe
    {
      get { lock (Padlock) return pipe; }
      set { lock (Padlock) pipe = value; }
    }

    abstract protected void AsyncReadReady(IAsyncResult result);

    public virtual T Pop()
    {
      lock (Padlock) return pipe.Pop();
    }

    public virtual bool Push(T element)
    {
      lock (Padlock) return pipe.Push(element);
    }

    public virtual bool FromConfig(ModuleConfig config)
    {
      throw new NotImplementedException();
    }

    public virtual ModuleConfig ToConfig()
    {
      throw new NotImplementedException();
    }

    public virtual object PopObject()
    {
      lock (Padlock) return pipe.PopObject();
    }

    public virtual bool PushObject(object element)
    {
      lock (Padlock) return pipe.PushObject(element);
    }
  }
}
