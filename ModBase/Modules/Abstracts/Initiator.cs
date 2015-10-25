using ModBase.Configuration.Properties;
using ModBase.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModBase.Modules
{
  public abstract class Initiator : IInitiator
  {
    #region constructors
    public Initiator()
    {
      AutoCleanup = false;
      IsInitialized = false;
      AutoInitialize = false;
    }

    #endregion

    #region properties
    public virtual bool AutoCleanup
    {
      get;
      set;
    }

    public virtual bool AutoInitialize
    {
      get;
      set;
    }

    public virtual bool IsInitialized
    {
      get;
      set;
    }
    #endregion

    #region functions
    public virtual bool Cleanup()
    {
      if (IsInitialized)
      {
        //!TODO clean all properties 
        IsInitialized = false;
        return !IsInitialized;
      }

      return false;
    }

    public virtual bool Initialize()
    {
      if (IsInitialized && AutoCleanup)
        Cleanup();

      if (!IsInitialized)
      {
        Type type = this.GetType();
        PropertyInfo[] properties = type.GetProperties();
        if (InitializeProperties(properties.GetEnumerator(), true)) {
          if(InitializeProperties(properties.GetEnumerator()))
            IsInitialized = true;
        }

        return IsInitialized;
      }

      return false;
    }
    protected virtual bool InitializeProperty(PropertyInfo property)
    {
      bool result = false;
      if (property != null)
      {
        Type propertyType = property.PropertyType;
        ConfigureAttribute configure = property.GetCustomAttribute(typeof(ConfigureAttribute)) as ConfigureAttribute;
        MethodInfo getMethod = property.GetGetMethod();
        object value = null;
        if((getMethod != null) && (getMethod.GetParameters().Count() == 0))
          value = property.GetValue(this);

        if ((configure == null) && (value != null)) {
          if((value as IInitiator) != null)
            (value as IInitiator).Initialize();          
        } else if ((configure != null) && (value == null)) {
          if (configure.AutoInit())
            if (configure.InitTypeTemplateCnt > 0)
              InitializeTemplateTypes(configure, property);
            else
              property.SetValue(this, Activator.CreateInstance(configure.InitType));
          else
            property.SetValue(this, configure.DefaultValue);
          value = property.GetValue(this);
        }
        if ((value as IInitiator) != null)
          ((IInitiator)value).Initialize();        
        result = true;
      }
      return result;
    }

    protected virtual bool InitializeProperties(System.Collections.IEnumerator enumerator, bool preInit = false)
    {
      bool result = true;
      while (enumerator.MoveNext() && result)
      {
        PropertyInfo current = enumerator.Current as PropertyInfo;
        ConfigureAttribute configAttribute = (current.GetCustomAttribute(typeof(ConfigureAttribute)) as ConfigureAttribute);
        if (preInit && (configAttribute != null) && configAttribute.PreInit)
          result = InitializeProperty(enumerator.Current as PropertyInfo);        
        else if (((!preInit) && (configAttribute == null)) || ((configAttribute!=null) && (!configAttribute.PreInit)))          
          result = InitializeProperty(enumerator.Current as PropertyInfo);                
      }
      return result;
    }

    protected virtual bool InitializeTemplateTypes(ConfigureAttribute config, PropertyInfo property)
    {
      bool result = false;
      if (this.GetType().GenericTypeArguments.Count() >= (config.InitTypeTemplateCnt - config.InitTypeParameters.Count()))
      {
        int i = 0;
        while (config.InitTypeParameters.Count() < config.InitTypeTemplateCnt)
        {
          config.InitTypeParameters.Add(this.GetType().GenericTypeArguments[i]);
          i++;
        }
        config.InitType = config.InitType.MakeGenericType(config.InitTypeParameters.ToArray());
        property.SetValue(this, Activator.CreateInstance(config.InitType));
        result = true;
      }
      return result;
    }
    
    #endregion
  }
}
