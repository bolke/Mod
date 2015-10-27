using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Interfaces.Pipes
{
  public interface IQueuePipe<T>: ILockable, IConfigurable, IBasePipe
  {
    T Pop();
    bool Push(T element);
  }
}
