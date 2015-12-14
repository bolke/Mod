using Mod.Configuration.Modules;
using Mod.Configuration.Section;
using Mod.Interfaces;
using Mod.Modules;
using Mod.Modules.Abstracts;
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
      ModConfigSectionReader loader = new ModConfigSectionReader();
      loader.Load();

      for(int i = 0; i < loader.ModuleInstances.Count;i++ )
      {
        ModuleConfig module = loader.ModuleInstances.ElementAt(i).Value;

        if(module != null)
        {
          List<ModuleConfig> modules = new List<ModuleConfig>();
          if ((module.Instance as Initiator) != null)
          {
            ModuleConfig mc = (module.Instance as Initiator).ToConfig();
            modules.Add(mc);
          }
        }
      }
      Console.ReadLine();
    }
  }
}
