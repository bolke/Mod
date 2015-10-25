using ModBase.Configuration.Properties;
using ModBase.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBase.Interfaces
{
  public interface IJob : IExecutable, ILockable, IConfigurable, IDisposable
  {  
    [Configure(DefaultValue = 0)]
    uint CrashCount
    {
      get;
      set;
    }

    [Configure(DefaultValue=null)]
    Delegate Delegated
    {
      get;
      set;
    }

    [Configure(DefaultValue=null)]
    object[] DelegateParameters
    {
      get;
      set;
    }

    [Configure(DefaultValue = false)]
    bool SaveOnFinish
    {
      get;
      set;
    }

    [Configure(DefaultValue = "")]
    string Label
    {
      get;
      set;
    }    

    bool Work();
  }
}
