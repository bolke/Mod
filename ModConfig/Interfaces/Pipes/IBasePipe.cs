using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Interfaces
{
  public interface IBasePipe
  {
    object PopObject();
    bool PushObject(object element);
  }
}
