using Mod.Configuration.Properties;
using Mod.Interfaces;
using Mod.Interfaces.Containers;
using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml;

namespace Mod.Configuration.Modules
{
  public class ModuleConfig : ConfigurationElement
  {
    #region variables
    private Guid uniqueId;
    #endregion

    #region constructors
    public ModuleConfig()
    {
      Parent = null;
      uniqueId = Guid.NewGuid();
    }
    #endregion

    #region properties
    [ConfigurationProperty("Properties")]
    [ConfigurationCollection(typeof(PropertyConfig), AddItemName = "Property")]
    public PropertyConfigCollection PropertyConfigCollection
    {
      get
      {
        return (PropertyConfigCollection)base["Properties"];
      }
    }
    public object Instance
    {
      get;
      set;
    }

    [ConfigurationProperty("Modules")]
    [ConfigurationCollection(typeof(ModuleConfig), AddItemName = "Module")]
    public ModuleConfigCollection ModuleConfigCollection
    {
      get
      {
        return (ModuleConfigCollection)base["Modules"];
      }
    }

    public ModuleConfig Parent
    {
      get;
      set;
    }
    
    [ConfigurationProperty("Type", IsRequired = true)]
    public virtual String Type
    {
      get { return (String)this["Type"]; }
      set { this["Type"] = value; }
    }

    public Guid UniqueId
    {
      get { return uniqueId; }
    }
    
    #endregion

    #region functions
    #region ConfigurationElement
    public virtual bool HasProperty(string name, bool whitespaceAllowed = true)
    {
      bool result = this.Properties.Contains(name);
      if (result && (!whitespaceAllowed))
        result = !string.IsNullOrWhiteSpace(this[name] as string);
      return result;
    }

    public virtual bool ProxySerializeElement(XmlWriter writer, bool serializeCollectionKey)
    {
      return SerializeElement(writer, serializeCollectionKey);
    }

    public virtual void ProxyDeserializeElement(XmlReader reader, bool serializeCollectionKey)
    {
      DeserializeElement(reader, serializeCollectionKey);
    }

    public virtual object GetConfigurationProperty(string name)
    {
      object result = this[name];
      return result;
    }

    protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
    {
      base.DeserializeElement(reader, serializeCollectionKey);
    }

    protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
    {
      Properties.Add(new ConfigurationProperty(name, typeof(String)));
      this[name] = value;
      return true;
    }

    protected override object OnRequiredPropertyNotFound(string name)
    {
      if (HasProperty("Key") && (this["Key"] != null))
      {
        if (!String.IsNullOrWhiteSpace(this["Key"] as string))
        {
          this[name] = "";
          return this[name];
        }
      }

      return base.OnRequiredPropertyNotFound(name);
    }
    #endregion
    public virtual bool Initialize()
    {
      bool result = true;
      if (Instance != null)
      {
        IEnumerator properties = this.ModuleConfigCollection.GetEnumerator();
        while (properties.MoveNext())
          result = result && SetInstanceProperty(properties.Current as ModuleConfig);
        if (typeof(IInitiator).IsAssignableFrom(Instance.GetType()))
        {
          if(!((IInitiator)Instance).IsInitialized)
          {
            ((IInitiator)Instance).Initialize();
          }
          result = result && ((IInitiator)Instance).IsInitialized;
        }

        if (result)
          SetConfigProperties();
      }

      return result;
    }

    public virtual Boolean IsInstance()
    {
      return HasProperty("Type") && !String.IsNullOrWhiteSpace(this["Type"] as string);
    }

    public virtual Boolean IsObjectContainer(Type instanceType = null, Type contentType = null)
    {
      Boolean result = false;
      if (Instance != null)
      {
        if (instanceType == null)
          instanceType = Instance.GetType();
        Type[] interfaces = instanceType.GetInterfaces();
        for (int i = 0; i < interfaces.Length; i++)
        {
          if (interfaces[i].Name.Equals(typeof(IObjectContainer).Name) || IsObjectContainer(interfaces[i], contentType))
          {
            if (contentType == null)
              return true;
            else
            {
              return false;
            }
          }
        }
      }

      return result;
    }

    public virtual bool IsProperty()
    {
      return HasProperty("Property", false);
    }

    public virtual Boolean IsReference()
    {
      return (HasProperty("Key") && !HasProperty("Type"));
    }

    public virtual bool SetConfigProperties()
    {
      bool result = true;
      if (Instance != null)
      {
        Type type = Instance.GetType();
        IEnumerator enumerator = Properties.GetEnumerator();
        while (enumerator.MoveNext())
        {
          ConfigurationProperty configProperty = enumerator.Current as ConfigurationProperty;
          PropertyInfo property = type.GetProperty(configProperty.Name);
          if (property != null)
          {
            ConfigureAttribute configAttribute = property.GetCustomAttribute(typeof(ConfigureAttribute)) as ConfigureAttribute;
            if (configAttribute != null)
            {
              if (property.PropertyType != typeof(string) && (string.IsNullOrWhiteSpace(this[configProperty] as string)))
                continue;
              object configValue = Convert.ChangeType(this[configProperty.Name], property.PropertyType);
              if (!(configAttribute.DefaultValue.Equals(configValue)))
                property.SetValue(Instance, configValue);
            }
          }
        }
      }

      return result;
    }
    
    protected virtual bool SetProperty(PropertyConfig member)
    {
      bool result = false;
      Type instanceType = Instance.GetType();
      PropertyInfo[] properties = instanceType.GetProperties();
      IEnumerator enumerator = properties.GetEnumerator();
      while (enumerator.MoveNext())
      {
        PropertyInfo propertyInfo = enumerator.Current as PropertyInfo;
        if (propertyInfo.Name == member.Name)
        {          
          object value = propertyInfo.GetValue(member.Parent.Instance);
          propertyInfo.SetValue(Instance,value);
          result = true;
          break;
        }
      }
      return result;
    }

    public virtual bool SetFullProperties()
    {
      bool result = true;
      IEnumerator enumerator = PropertyConfigCollection.GetEnumerator();
      while (enumerator.MoveNext())
      {
        PropertyConfig propertyConfig = enumerator.Current as PropertyConfig;
        if (propertyConfig != null)
        {
          if (propertyConfig.Original || string.IsNullOrWhiteSpace(propertyConfig.Key))
          {          
            PropertyInfo property = Instance.GetType().GetProperty(propertyConfig.Name);
            ConfigureAttribute configAttribute = property.GetCustomAttribute(typeof(ConfigureAttribute)) as ConfigureAttribute;
            if (configAttribute != null)
            {
              if (property.PropertyType != typeof(string) || (!string.IsNullOrWhiteSpace(propertyConfig.Value)))
              {
                object configValue = Convert.ChangeType(propertyConfig.Value, property.PropertyType);
                if (!(configAttribute.DefaultValue.Equals(configValue)))
                  property.SetValue(Instance, configValue);
              }
            }

            IEnumerator members = propertyConfig.PropertyMembers.GetEnumerator();
            while (members.MoveNext())
            {
              PropertyConfig member = members.Current as PropertyConfig;
              member.Parent.SetProperty(propertyConfig);
            }
          }
        }
      }
      return result;
    }

    public virtual bool SetInstanceProperty(ModuleConfig propertyContent)
    {
      bool result = false;
      if ((Instance != null) && (propertyContent != null))
      {
        if (propertyContent.IsProperty())
        {
          Type instanceType = Instance.GetType();
          PropertyInfo[] properties = instanceType.GetProperties();
          for (int i = 0; i < properties.Length; i++)
          {
            PropertyInfo property = properties[i];
            if (property.Name.Equals(propertyContent["Property"]))
            {
              property.SetValue(Instance, propertyContent.Instance);
              result = true;
              break;
            }
          }
        }
        else
          return true;
      }

      return result;
    }
    #endregion
    
  }
}
