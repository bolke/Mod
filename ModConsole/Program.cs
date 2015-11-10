using Mod.Configuration.Section;
using Mod.Modules;
using Mod.Modules.Lines;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModConsole
{
  class Program
  {
    static void Main(string[] args)
    {
      /*
      MemoryStream mem = new MemoryStream();
      BinaryFormatter bif = new BinaryFormatter();
      bif.Serialize(mem, "cake");
      bif.Serialize(mem, "fart");
      mem.Position = 0;
      object k = bif.Deserialize(mem);
      object l = bif.Deserialize(mem);
      */
      SerialPort sp = new SerialPort("COM11");
      sp.Open();

      PipeStream ps = new PipeStream();
      //ps.Stream = new MemoryStream();
      ps.Stream = sp.BaseStream;
      ps.Initialize();
      while (true)
      {

        ps.PushObject("cake");
        Thread.Sleep(1000);
        ps.ReadNext();
        Thread.Sleep(1000);
        object o = ps.PopObject();
        if(o != null)
          Console.WriteLine(o.ToString());
      }
      //ModConfigSectionReader loader = new ModConfigSectionReader();
      //loader.LoadAndRun();
      Console.ReadLine();
    }
  }
}
