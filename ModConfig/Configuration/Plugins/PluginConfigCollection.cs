using System.Configuration;

namespace Mod.Configuration.Plugins
{
    public class PluginConfigCollection : ConfigurationElementCollection
    {
        public PluginConfig this[int index]
        {
            get { return (PluginConfig)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PluginConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PluginConfig)(element)).FilePath;
        }
    }
}
