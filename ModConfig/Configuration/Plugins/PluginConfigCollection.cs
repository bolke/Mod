using System.Collections.Generic;
using System.Configuration;

namespace Mod.Configuration.Plugins
{
    public class PluginConfigCollection: ConfigurationElementCollection
    {
        private List<PluginConfig> _elements = new List<PluginConfig>();

        public PluginConfig this[int index]
        {
            get { return (PluginConfig)BaseGet(index); }
            set
            {
                if((index >= 0) && (index < Count))
                {
                    if(BaseGet(index) != null)
                    {
                        BaseRemoveAt(index);
                    }
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            PluginConfig result = new PluginConfig();
            _elements.Add(result);
            return result;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PluginConfig)(element)).FilePath;
        }
    }
}
