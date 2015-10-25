using ModBase.Configuration;
using ModBase.Configuration.Modules;
using ModBase.Configuration.Plugins;
using ModBase.Configuration.Section;
using ModBase.Configuration.Types;
using ModBase.Interfaces;
using ModBase.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModConsole
{  
  class Program
  {
    public void TickTest()
    {
      Random rand = new Random();
      int ttick = 0;
      uint tick = 0;
      uint tock = 0;
      uint execute = 1;
      uint interval = 0x10000000;
      int i = 0;

      while (true)
      {
        ttick = ttick + rand.Next(1, 100000000);
        if (ttick < 0)
          tick = Convert.ToUInt32((UInt32)ttick);
        else
          tick = Convert.ToUInt32(ttick);

        if (execute < tick)
        {
          i++;
          System.Console.WriteLine("EXECUTE " + execute + ":" + tick + ":" + tock);
          execute += interval;
        }

        if (tock > tick)
        {
          System.Console.WriteLine("TOCK    " + execute + ":" + tick + ":" + tock);
          if (execute > tock)
            execute = execute - tock;
          else
            execute = tick;
          i = 0;
        }
        tock = tick;
      }
      Console.ReadLine();
    }

    static void Main(string[] args)
    {
      List<Lockable> lockables = new List<Lockable>();

      ModConfigSectionLoader loader = new ModConfigSectionLoader();
      if (loader.Load())      
        loader.Run();

      Dictionary<Guid, ModuleConfig>.Enumerator enumerator = loader.ModuleInstances.GetEnumerator();
      while (enumerator.MoveNext())
      {   
        object item = ((ModuleConfig)enumerator.Current.Value).Instance;
        if (item as Lockable != null)
          lockables.Add(item as Lockable);
        Console.WriteLine(item.ToString());
      }
      Console.ReadLine();
    }
  }
}
