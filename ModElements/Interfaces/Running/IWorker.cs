using Mod.Configuration.Properties;
using Mod.Interfaces.Pipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Interfaces
{
  public interface IWorker<T> : IJob
  {
    [Configure(IsRequired=true)]
    IPipe<T> Pool
    {
      get;
      set;
    }

    [Configure(IsRequired = true)]
    IPipe<T> Finished
    {
      get;
      set;
    }
  }
}
