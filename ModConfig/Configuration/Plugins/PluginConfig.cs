using Mod.Configuration.Properties;
using Mod.Configuration.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Runtime.Remoting;
using System.Xml;

namespace Mod.Configuration.Plugins
{

    public class PluginConfig: ConfigurationElement
    {
        #region variables
        private TypeFactory typeFactory = TypeFactory.GetInstance();
        private Assembly assembly = null;
        #endregion

        #region properties
        [ConfigurationProperty("Path", DefaultValue = "./")]
        public virtual String Path
        {
            get { return (String)this["Path"]; }
            set { this["Path"] = value; }
        }

        [ConfigurationProperty("File", IsRequired = true)]
        public virtual String File
        {
            get { return (String)this["File"]; }
            set { this["File"] = value; }
        }

        public virtual String FilePath
        {
            get
            {
                String result = (System.AppDomain.CurrentDomain.BaseDirectory + (Path)).Replace("\\\\", "\\");
                result = new Uri(result).LocalPath;
                if(result.EndsWith("/") || result.EndsWith("\\"))
                    return result + File;
                return result + "/" + File;
            }
        }

        public virtual Assembly Assembly
        {
            get
            {
                if(assembly == null)
                {
                    try
                    {
                        assembly = Assembly.LoadFrom(FilePath);
                        if(assembly != null)
                            typeFactory.PluginAssemblies.Add(assembly);
                    }
                    catch
                    {
                        assembly = null;
                    }
                }
                return assembly;
            }
            set
            {
                if(assembly == null)
                {
                    assembly = value;
                    if(assembly != null)
                        typeFactory.PluginAssemblies.Add(assembly);
                }
            }
        }
        #endregion

        #region functions
        public virtual void ProxyDeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            DeserializeElement(reader, serializeCollectionKey);
        }

        public virtual bool ProxySerializeElement(XmlWriter writer, bool serializeCollectionKey)
        {
            return SerializeElement(writer, serializeCollectionKey);
        }
        #endregion
    }
}
