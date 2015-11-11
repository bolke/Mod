using Mod.Configuration.Section;
using Mod.Modules;
using Mod.Modules.Combinations;
using Mod.Modules.EndPoints;
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
      Pipe<String> pis = new Pipe<String>();
      Pipe<String> pis2 = new Pipe<String>();

      SerialPort sp = new SerialPort("COM3");
      SerialPort sp2 = new SerialPort("COM7");
      sp2.Open();
      sp.Open();

      PipeStream ps = new PipeStream();
      PipeStream ps2 = new PipeStream();
      ps2.Stream = sp2.BaseStream;
      ps.Stream = sp.BaseStream;

      pis.BasePipe = ps;
      pis2.BasePipe = ps2;

      pis.Initialize();
      pis2.Initialize();

      ps2.Initialize();
      ps.Initialize();

      int i = 0;
 
      while (true)
      {
        i++;
        pis.PushObject("cake");
        pis2.PushObject("is a lie");             
        Thread.Sleep(100);    
        if (i >= 10)
        {
          Thread.Sleep(100);    
          break;
        }
      }

      sp.Close();
      sp2.Close();

      //ModConfigSectionReader loader = new ModConfigSectionReader();
      //loader.LoadAndRun();

      i = 0;
      while (i < 100)
      {
        object o = pis2.Pop();
        if (o != null)
          Console.WriteLine(o.ToString());
        o = pis.Pop();
        if (o != null)
          Console.WriteLine(o.ToString());
        i++;
      }
      Console.ReadLine();
    }
  }
}
