using Mod.Configuration.Properties;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Interfaces.Pipes
{
  public interface IPipe<T> : IQueuePipe<T>, IList<T>
  {    
  }
}
