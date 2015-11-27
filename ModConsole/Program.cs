using Mod.Configuration.Modules;
using Mod.Configuration.Section;
using Mod.Interfaces;
using Mod.Modules;
using Mod.Modules.Combinations;
using Mod.Modules.EndPoints;
using Mod.Modules.Lines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
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

      SerialPort sp2 = new SerialPort("COM12");
      SerialPort sp1 = new SerialPort("COM11");

      PipeStream ps = new PipeStream();
      PipeStream ps2 = new PipeStream();

      sp1.Open();
      sp2.Open();
      
      ps.AutoOpen = true;
      ps2.AutoOpen = true;

      ps2.Stream = sp2.BaseStream;
      ps.Stream = sp1.BaseStream;//.GetStream();

      pis.BasePipe = ps;
      pis2.BasePipe = ps2;

      pis.Initialize();
      pis2.Initialize();

      ps2.Initialize();
      ps.Initialize();

      /*while (true)
      {
        ps2.PushObject(ps.PopObject());
        ps.PushObject(ps2.PopObject());
        Thread.Sleep(100);
      }*/

      ModConfigSectionReader loader = new ModConfigSectionReader();
      loader.Load();

      for(int i = 0; i < loader.ModuleInstances.Count;i++ )
      {
        ModuleConfig module = loader.ModuleInstances.ElementAt(i).Value;

        if(module != null)
        {
          if((module.Instance as IRunnable) != null)
          {
            (module.Instance as IRunnable).Start();
          }
        }
      }
      Console.ReadLine();
    }
  }
}
