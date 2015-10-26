using ModBase.Configuration.Properties;
using ModBase.Interfaces.Pipes;
using ModBase.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBase.Interfaces
{
  public interface IWorker<T> : IJob
  {
    [Configure(InitType = typeof(Pipe<>))]
    IPipe<T> Pool
    {
      get;
      set;
    }

    [Configure(InitType = typeof(Pipe<>))]
    IPipe<T> Finished
    {
      get;
      set;
    }
  }
}
