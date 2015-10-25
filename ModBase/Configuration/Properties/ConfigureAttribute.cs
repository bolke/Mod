using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ModBase.Configuration.Properties
{
  public class ConfigureAttribute : Attribute
  {
    private List<Type> initTypeParameters = new List<Type>();
    private Type initType = null;

    public ConfigureAttribute([CallerMemberName] string callerName = "")
    {
      CallerName = callerName;
      IsRequired = false;
      DefaultValue = null;
      InitType = null;
      PreInit = false;
      Key = "";
    }

    public virtual List<Type> InitTypeParameters
    {
      get
      {
        return initTypeParameters;
      }
    }

    public virtual String CallerName
    {
      get;
      set;
    }

    public virtual bool IsRequired
    {
      get;
      set;
    }

    public virtual bool PreInit
    {
      get;
      set;
    }

    public virtual object DefaultValue
    {
      get;
      set;
    }

    public virtual string Key
    {
      get;
      set;
    }

    public virtual Type InitType
    {
      get { return initType; }
      set
      {
        InitTypeTemplateCnt = 0;
        if (value != null)
        {
          string name = value.Name;
          if (name.Contains("`"))
          {
            initTypeParameters.Clear();
            InitTypeTemplateCnt = Convert.ToInt32(name.Substring(name.IndexOf("`") + 1));
            initTypeParameters.AddRange(value.GenericTypeArguments);
          }
        }
        initType = value;
      }
    }

    public virtual int InitTypeTemplateCnt
    {
      get;
      set;
    }

    public virtual bool AutoInit()
    {
      return (InitType != null) && (!IsRequired);
    }
  }
}