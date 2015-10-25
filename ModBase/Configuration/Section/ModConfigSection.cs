using ModBase.Configuration.Modules;
using ModBase.Configuration.Plugins;
using System.Configuration;

namespace ModBase.Configuration.Section
{
    public class ModConfigSection : ConfigurationSection
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
    }
}
