using Mod.Configuration.Modules;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Mod.Configuration.Properties
{
    public class PropertyConfig: ConfigurationElement
    {
        #region variables
        private Guid uniqueId;
        private List<PropertyConfig> propertyMembers;
        #endregion

        #region constructors
        public PropertyConfig()
        {
            Parent = null;
            OriginalConfig = null;
            uniqueId = Guid.NewGuid();
            propertyMembers = new List<PropertyConfig>();
        }
        #endregion

        #region properties
        [ConfigurationProperty("Name", IsRequired = true)]
        public virtual String Name
        {
            get { return (String)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("Type", IsRequired = false, DefaultValue = "")]
        public virtual String Type
        {
            get { return (String)this["Type"]; }
            set { this["Type"] = value; }
        }

        [ConfigurationProperty("Value", IsRequired = false, DefaultValue = "")]
        public virtual String Value
        {
            get { return (String)this["Value"]; }
            set { this["Value"] = value; }
        }

        [ConfigurationProperty("Original", IsRequired = false, DefaultValue = false)]
        public virtual bool Original
        {
            get { return (bool)this["Original"]; }
            set { this["Original"] = value; }
        }

        [ConfigurationProperty("Key", IsRequired = false, DefaultValue = "")]
        public virtual String Key
        {
            get { return (String)this["Key"]; }
            set { this["Key"] = value; }
        }

        public Guid UniqueId
        {
            get { return uniqueId; }
        }

        public ModuleConfig Parent
        {
            get;
            set;
        }

        public PropertyConfig OriginalConfig
        {
            get;
            set;
        }

        public List<PropertyConfig> PropertyMembers
        {
            get { return propertyMembers; }
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

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);
        }

        public virtual Boolean IsReference()
        {
            return !String.IsNullOrWhiteSpace(Key) && (!Original);
        }

        #endregion

    }
}
