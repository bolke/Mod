using ModBase.Configuration.Properties;
using ModBase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBase.Modules
{
  public class Job : Executable, IJob
  {
    #region variables
    private Delegate delegated = null;
    private object[] delegateParameters = null;    
    private uint crashCount = 0;    
    private bool saveOnFinish = false;    
    private string label = "";    
    #endregion

    #region properties    
    
    [Configure(DefaultValue = 0)]
    public virtual uint CrashCount
    {
      get
      {
        lock (Padlock) return crashCount;
      }
      set
      {
        lock (Padlock) crashCount = value;
      }
    }

    public virtual Delegate Delegated
    {
      get
      {
        lock (Padlock) return delegated;
      }
      set
      {
        lock (Padlock) delegated = value;
      }
    }

    public virtual object[] DelegateParameters
    {
      get
      {
        lock (Padlock) return delegateParameters;
      }
      set
      {
        lock (Padlock) delegateParameters = value;
      }
    }

    [Configure(DefaultValue = false)]
    public virtual bool SaveOnFinish
    {
      get
      {
        lock (Padlock) return saveOnFinish;
      }
      set
      {
        lock (Padlock) saveOnFinish = value;
      }
    }

    [Configure(DefaultValue = "")]
    public virtual string Label
    {
      get
      {
        lock (Padlock) return label;
      }
      set
      {
        lock (Padlock) label = value;
      }
    }

    #endregion

    #region functions
    public override bool Initialize()
    {
      if (base.Initialize())
      {
        delegated = null;
        delegateParameters = null;        
        crashCount = 0;
        return true;
      }
      return false;
    }

    public override bool Cleanup()
    {
      if (base.Cleanup())
      {
        delegated = null;
        delegateParameters = null;        
        crashCount = 0;
        return true;
      }
      return false;
    }   


    public override string ToString()
    {
      string result = Label;
      if (string.IsNullOrWhiteSpace(result))
        return base.ToString() + ExecuteTick;
      return result;
    }

    public sealed override bool Execute()
    {
      bool result = false;
      if (base.Execute())
      {
        result = Work();
      }
      return result;
    }

    public virtual bool Work()
    {
      System.Console.WriteLine("WORK WORK");
      return false;
    }

    public override System.Configuration.ConfigurationElement ToConfig()
    {
      throw new NotImplementedException();
    }

    public override bool FromConfig(System.Configuration.ConfigurationElement config)
    {
      throw new NotImplementedException();
    }

    public virtual void Dispose()
    {
    }

    #endregion

  }
}
