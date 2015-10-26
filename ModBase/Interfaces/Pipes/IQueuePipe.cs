using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBase.Interfaces.Pipes
{
  public interface IQueuePipe<T>: ILockable, IConfigurable, IObjectPipe
  {
    T Pop();
    bool Push(T element);
  }
}
