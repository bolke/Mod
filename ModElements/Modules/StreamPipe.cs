using Mod.Configuration.Properties;
using Mod.Interfaces.Pipes;
using Mod.Modules.Abstracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Modules
{
  public class StreamPipe<T>: Lockable, IQueuePipe<T>
  {
    private Stream stream = null;
    private Boolean doRead = true;
    private Boolean doWrite = true;
    private ConcurrentQueue<byte[]> dataRead = null;
    private ConcurrentQueue<byte[]> dataWrite = null;
    private int readChunkSize = 4096;
    private int writeChunkSize = 4096;
    private byte[] readBuffer = null;    
    
    [Configure(IsRequired=true)]
    public virtual Stream Stream
    {
      get {lock(Padlock) return stream; }
      set {lock(Padlock) stream = value; }
    }

    [Configure(InitType=typeof(ConcurrentQueue<byte[]>))]
    public virtual ConcurrentQueue<byte[]> DataRead
    {
      get { return dataRead; }
      protected set {dataRead = value;}
    }

    [Configure(InitType=typeof(ConcurrentQueue<byte[]>))]
    public virtual ConcurrentQueue<byte[]> DataWrite
    {
      get { return dataWrite; }
      protected set { dataWrite = value; }
    }

    [Configure(DefaultValue=4096)]
    public virtual int ReadChunkSize
    {
      get { return readChunkSize; }
      set { readChunkSize = value; }
    }

    [Configure(DefaultValue=4096)]
    public virtual int WriteChunkSize
    {
      get { return writeChunkSize; }
      set { writeChunkSize = value; }
    }

    public override bool Initialize()
    {
      if(base.Initialize())
      {
        readBuffer = new byte[readChunkSize];
        return true;
      }
      return false;
    }

    public virtual T Pop()
    {
      T result = default(T);
      if(typeof(T) == typeof(byte[]))
      {
        byte[] data;
        if(dataWrite.TryDequeue(out data))
        {
          result = (T)(object)data;
        }
      }
      return result;
    }

    public virtual bool Push(T element)
    {
      if(typeof(T) == typeof(byte[]))
      {
        dataWrite.Enqueue(element as byte[]);
        return true;
      }
      return false;
    }

    public virtual object PopObject()
    {
      return Pop();
    }

    public virtual bool PushObject(object element)
    {
       return Push((T)element);
    }

    public virtual Configuration.Modules.ModuleConfig ToConfig()
    {
      throw new NotImplementedException();
    }

    public virtual bool FromConfig(Configuration.Modules.ModuleConfig config)
    {
      throw new NotImplementedException();
    }

    protected virtual void ReadNext()
    {
      lock(Padlock)
      {
        if(doRead)
        {
          doRead = false;
          stream.BeginRead(readBuffer, 0, readChunkSize, new AsyncCallback(CompleteRead), null);
        }
      }
    }

    protected virtual void WriteNext(){
      lock(Padlock)
      {
        if(doWrite)
        {
          byte[] dout = null;
          if(dataWrite.TryDequeue(out dout))
          {
            doWrite = false;
            stream.BeginWrite(dout, 0, dout.Count(), new AsyncCallback(CompleteWrite), null);
          }
        }
      }
    }

    protected virtual void CompleteWrite(IAsyncResult asyncResult)
    {
      try
      {
        stream.EndWrite(asyncResult);
        doWrite = true;
        WriteNext();
      }
      catch
      {
        Console.WriteLine("catch write");
      }
    }

    protected virtual void CompleteRead(IAsyncResult asyncResult)
    {
      try
      {
        int read = stream.EndRead(asyncResult);
        byte[] newData = new byte[read];
        Array.Copy(readBuffer, newData, read);
        dataRead.Enqueue(readBuffer);
        doRead = true;
        ReadNext();
      }
      catch
      {
        Console.WriteLine("catch read");
      }
    }

  }
}
