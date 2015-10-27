using Mod.Configuration.Modules;
using Mod.Configuration.Plugins;
using System.Configuration;

namespace Mod.Configuration.Section
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
