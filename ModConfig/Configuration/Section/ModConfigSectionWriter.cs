using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Configuration.Section
{
  public class ModConfigSectionWriter
  {
    public ModConfigSection ModConfigSection { get; set; }

    public ModConfigSectionWriter()
    {
      ModConfigSection = null;
    }

    public Boolean Save(string sectionName = "ModConfigSection")
    {
      return false;
    }

    public Boolean AddElement(object element)
    {
      return false;
    }
  }
}
