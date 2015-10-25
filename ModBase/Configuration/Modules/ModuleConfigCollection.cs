using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ModBase.Configuration.Modules
{
  public class ModuleConfigCollection : ConfigurationElementCollection
  {
    private List<ModuleConfig> _elements = new List<ModuleConfig>();

    public virtual ModuleConfig this[int index]
    {
      get { return (ModuleConfig)BaseGet(index); }
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
      _elements.Add(new ModuleConfig());
      return _elements.Last();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return ((ModuleConfig)element).UniqueId;
    }
  }
}
