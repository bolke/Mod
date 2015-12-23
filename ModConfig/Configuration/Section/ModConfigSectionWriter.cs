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
                    directory = directory.Replace('\\','/');
                    System.IO.Directory.CreateDirectory(directory);
                    return true;
                }catch
                {
                    //stuff
                }
            }
            return false;
        }

        public virtual Boolean Save(string file, string directory)
        {
            if(ModConfigSection != null)
            {
                if(CreateDirectory(directory)){
                    System.Text.StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                    sb.AppendLine("<configuration>");
                    sb.AppendLine("<section name=\"ModConfigSection\" type=\"Mod.Configuration.Section.ModConfigSection, ModConfig\" />");
                    sb.AppendLine("</configuration>");
                    File.WriteAllText(directory + "temp.config",sb.ToString());

                    StringWriter sw = new StringWriter();
                    XmlWriter x = XmlWriter.Create(sw);

                    if(ModConfigSection.SerializeSection(x))
                        Console.WriteLine("poop");

                    string sss = sw.ToString();

                    ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                    configMap.ExeConfigFilename = "f:/ModConsole.exe.config";

                    System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                    config.SaveAs(directory + "sdfsfsd.config", ConfigurationSaveMode.Modified);
                 //   config.SaveAs(directory + newFile, ConfigurationSaveMode.Minimal);
                    return true;
                }
            }
            return false;
        }
    }
}
