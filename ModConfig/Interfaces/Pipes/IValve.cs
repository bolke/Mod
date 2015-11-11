using Mod.Configuration.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Interfaces.Pipes
{
  public interface IValve
  {
    bool IsOpen
    {
      get;
    }
    
    bool IsClosed
    {
      get;      
    }

    bool CanRead
    {
      get;
    }

    bool CanWrite
    {
      get;
    }

    [Configure(DefaultValue = true)]
    bool AutoOpen
    {
      get;
      set;
    }

    Boolean Open();
    Boolean Close();   
  }
}
