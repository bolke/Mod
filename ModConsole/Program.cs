using Mod.Configuration.Section;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModConsole
{
  class Program
  {
    static void Main(string[] args)
    {
      ModConfigSectionReader loader = new ModConfigSectionReader();
      loader.LoadAndRun();
      Console.ReadLine();
    }
  }
}
