using Mod.Configuration.Properties;
using Mod.Interfaces;
using Mod.Interfaces.Containers;
using Mod.Interfaces.Pipes;
using Mod.Modules.Abstracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Modules.Lines
{
  public class PipeStream: Lockable, IObjectContainer, IValve
  {
    private ConcurrentQueue<Object> toSend = new ConcurrentQueue<object>();
    private bool doWrite = true;
    private bool doRead = true;
    private Stream stream = null;
    private Stream readStream = null;
    private Stream writeStream = null;
    private bool binaryData = true;
    private BinaryFormatter formatter = null;
    private Type targetType = null;        
    private byte[] dRead = null;
    private byte[] dWrite = null;
    private int chunkSizeRead = 4096;
    private int chunkSizeWrite = 4096;
    private bool autoOpen = false;

    [Configure(DefaultValue=4096)]
    public virtual int ChunkSizeRead{
      get{lock(Padlock) return chunkSizeRead;}
      set{lock(Padlock) chunkSizeRead = value;}
    }

    [Configure(DefaultValue=true)]
    public virtual Boolean BinaryData
    {
      get { lock(Padlock) return binaryData; }
      set { lock(Padlock) binaryData = value; }
    }

    [Configure(DefaultValue=4096)]
    public virtual int ChunkSizeWrite{
      get{lock(Padlock) return chunkSizeWrite;}
      set{lock(Padlock) chunkSizeWrite = value;}
    }

    [Configure(InitType=typeof(MemoryStream))]
    public virtual Stream ReadStream
    {
      get { lock (Padlock) return readStream; }
      set { lock (Padlock) readStream = value; }
    }

    [Configure(InitType=typeof(MemoryStream))]
    public virtual Stream WriteStream
    {
      get { lock (Padlock) return writeStream; }
      set { lock (Padlock) writeStream = value; }
    }

    public virtual Type TargetType
    {
      get { lock (Padlock) return targetType; }
      set { lock (Padlock) targetType = value; }
    }

    [Configure(IsRequired=true)]
    public virtual Stream Stream
    {
      get { lock (Padlock) return stream; }
      set { lock (Padlock) stream = value; }
    }

    public override bool Initialize()
    {
      if (base.Initialize())
      {
        formatter = new BinaryFormatter();
        dWrite = new byte[chunkSizeWrite];
        dRead = new byte[chunkSizeRead];
        if(autoOpen)
          ReadNext();
        return true;
      }
      return false;
    }

    public virtual object PopObject()
    {
      lock (Padlock)
      {
        try
        {
          object result = formatter.Deserialize(readStream);
          return result;
        }
        catch
        {
        }
      }
      return null;
    }

    public virtual bool PushObject(object element)
    {
      if(element != null)
      {
        lock(Padlock)
        {
          Type t = element.GetType();
          if(t.IsSerializable)
          {
            toSend.Enqueue(element);
            WriteNext();
            return true;
          }
        }
      }
      return false;
    }

    protected virtual void WriteNext()
    {      
      lock (Padlock)
      {
        if (doWrite)
        {          
          object objectToSend = null;
          if (toSend.TryDequeue(out objectToSend))
          {
            doWrite = false;
            formatter.Serialize(writeStream, objectToSend);
            writeStream.Position = 0;
            int toWrite = writeStream.Read(dWrite, 0, dWrite.Length);                        
            stream.BeginWrite(dWrite,0,toWrite,new AsyncCallback(CompleteWrite), null);            
          }
        }
      }
    }

    public virtual void ReadNext()
    {      
      if (doRead)
      {          
        doRead = false;
        stream.BeginRead(dRead, 0, dRead.Length, new AsyncCallback(CompleteRead), null);
      }     
    }

    protected virtual void CompleteRead(IAsyncResult result)
    {
      try
      {
        int readCnt = Stream.EndRead(result);
        if(readCnt > 0)
        {
          long position = readStream.Position;
          readStream.Position = readStream.Length;
          readStream.Write(dRead, 0, readCnt);
          readStream.Position = position;
        }
        doRead = true;
        ReadNext();
      }
      catch
      {
      }
    }

    protected virtual void CompleteWrite(IAsyncResult result)
    {
      try
      {
        if (writeStream.Length > writeStream.Position)
        {
          int toWrite = writeStream.Read(dWrite, 0, dWrite.Length);
          stream.BeginWrite(dWrite, 0, toWrite, new AsyncCallback(CompleteWrite), null);          
        }
        else
        {
          writeStream.SetLength(0);
          doWrite = true;          
        }
      }
      catch
      {
      }
    }


    public virtual bool IsOpen
    {
      get { lock(Padlock) return stream!=null? stream.CanRead || stream.CanWrite: false; }
    }

    public virtual bool IsClosed
    {
      get { return !IsOpen; }
    }

    public virtual bool CanRead
    {
      get { lock(Padlock) return stream!=null?stream.CanRead:false; }
    }

    public virtual bool CanWrite
    {
      get { lock (Padlock) return stream != null ? stream.CanWrite : false; }
    }

    [Configure(DefaultValue = false)]
    public virtual bool AutoOpen
    {
      get
      {
        lock (Padlock) return autoOpen;
      }
      set
      {
        lock (Padlock) autoOpen = value;
      }
    }

    public virtual bool Open()
    {
      lock (Padlock)
      {
        if (stream != null)
        {
          ReadNext();
          return IsOpen;
        }
        return false;
      }
    }

    public virtual bool Close()
    {
      lock (Padlock)
      {
        if(stream!=null)
         stream.Close();
        return IsClosed;
      }
    }
  }
}
