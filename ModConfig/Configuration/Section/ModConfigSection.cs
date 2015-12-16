using Mod.Configuration.Modules;
using Mod.Configuration.Plugins;
using System;
using System.Configuration;
using System.Xml;

namespace Mod.Configuration.Section
{
  public class ModConfigSection: ConfigurationSection
  {
    [ConfigurationProperty("Plugins")]
    [ConfigurationCollection(typeof(PluginConfig), AddItemName = "Plugin")]
    public PluginConfigCollection PluginCollection
    {
      get
      {
        return (PluginConfigCollection)base["Plugins"];
      }
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

    public virtual void DeserializeSection(XmlReader reader)
    {
      base.DeserializeSection(reader);
    }

    public virtual bool SerializeSection(XmlWriter writer)
    {
      return base.SerializeToXmlElement(writer, "ModConfigSection");
    }

  }
}
