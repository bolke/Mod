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
using System.Configuration;
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

      ModuleConfigCollection modules = new ModuleConfigCollection();
      
      for(var i = 0; i < loader.ModuleInstances.Count;i++ )
      {
        ModuleConfig module = loader.ModuleInstances.ElementAt(i).Value;
        if(module != null)
        {
          if ((module.Instance as Initiator) != null)
          {
            modules[modules.Count] = (module.Instance as Initiator).ToConfig(true);
          }
        }
      }

      for(var i = 0; i < modules.Count; i++)
      {
        for(var j = 0; j < modules.Count; j++)
        {
          for(var k = 0; k < modules[i].ModuleConfigCollection.Count; k++) 
          {
            if(modules[i].ModuleConfigCollection[k].Instance == modules[j].Instance)
            {
              modules[i].ModuleConfigCollection[k] = modules[j];
            }
          }
        }
      }

      Console.ReadLine();
    }
  }
}
