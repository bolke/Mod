using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ModBase.Configuration.Properties
{
  public class PropertyConfigCollection:  ConfigurationElementCollection
  {
    private List<PropertyConfig> _elements = new List<PropertyConfig>();

    public PropertyConfig this[int index]
    {
      get { return (PropertyConfig)BaseGet(index); }
      set 
      {
        if ((index >= 0) && (index < Count))
        {
          if (BaseGet(index) != null)
            BaseRemoveAt(index);
        }
        BaseAdd(index, value);
      }
    }

    protected override bool IsElementName(string elementName)
    {
      return true;
    }

    protected override ConfigurationElement CreateNewElement()
    {
      _elements.Add(new PropertyConfig());
      return _elements.Last();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return ((PropertyConfig)element).UniqueId;
    }
  }
}
