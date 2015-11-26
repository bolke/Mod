using Mod.Configuration.Properties;
using Mod.Interfaces.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Interfaces.Pipes
{
  public interface IQueuePipe<T>: ILockable, IConfigurable, IObjectContainer
  {
    [Configure()]
    IObjectContainer BasePipe { get; set; }
    T Pop();
    bool Push(T element);
  }
}
