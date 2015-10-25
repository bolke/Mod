using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBase.Interfaces
{
  public interface IObjectPipe
  {
    object PopObject();
    bool PushObject(object element);
  }
}
