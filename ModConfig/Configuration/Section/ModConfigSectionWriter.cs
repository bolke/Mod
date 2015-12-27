using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Mod.Configuration.Section
{
    public class ModConfigSectionWriter
    {
        public ModConfigSection ModConfigSection { get; set; }

        public ModConfigSectionWriter()
        {
            ModConfigSection = null;
        }

        protected bool CreateDirectory(string directory)
        {
            if(!string.IsNullOrWhiteSpace(directory))
            {
                try
                {
                    directory = directory.Replace('\\', '/');
                    System.IO.Directory.CreateDirectory(directory);
                    return true;
                }
                catch
                {
                    //stuff
                }
            }
            return false;
        }

        public virtual Boolean Save(string file, string directory = "./")
        {
            if(ModConfigSection != null)
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.ConformanceLevel = ConformanceLevel.Auto;
                settings.Indent = true;
                settings.IndentChars = "    ";
                StringWriter stringWriter = new StringWriter();
                XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings);
                //if(ModConfigSection.SerializeElement(xmlWriter))
                if(ModConfigSection.SerializeSection(xmlWriter))
                {
                    xmlWriter.Flush();
                    if(!(directory.EndsWith("/") || directory.EndsWith("\\")))
                    {
                        directory += "/";
                    }
                    if(CreateDirectory(directory))
                    {
                        File.WriteAllText(directory + file, stringWriter.ToString());
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
