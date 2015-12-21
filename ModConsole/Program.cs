using Mod.Configuration.Modules;
using Mod.Configuration.Section;
using Mod.Interfaces;
using Mod.Modules;
using Mod.Modules.Abstracts;
using Mod.Modules.Combinations;
using Mod.Modules.EndPoints;
using Mod.Modules.Lines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ModConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            ModConfigSectionReader loader = new ModConfigSectionReader();
            loader.Load();

            ModuleConfigCollection modules = new ModuleConfigCollection();

            for(var i = 0; i < loader.ModuleInstances.Count; i++)
            {
                ModuleConfig module = loader.ModuleInstances.ElementAt(i).Value;
                if(module != null)
                {
                    if((module.Instance as Initiator) != null)
                    {
                        modules[modules.Count] = (module.Instance as Initiator).ToConfig(true);
                    }
                }
            }

            for(var i = 0; i < modules.Count; i++)
            {
                for(var j = 0; j < modules.Count; j++)
                {
                    for(var k = 0; k < modules[i].ModuleConfigCollection.Count; k++)
                    {
                        if(modules[i].ModuleConfigCollection[k].Instance == modules[j].Instance)
                        {
                            //          modules[i].ModuleConfigCollection[k] = modules[j];
                        }
                    }
                }
            }
             */
            /*
            XmlWriter writer = XmlWriter.Create(@"f:/file.xml");
      
            ModConfigSection mcf = new ModConfigSection();
            for(var i = 0; i < modules.Count; i++)
              mcf.ModuleConfigCollection[i] = modules[i];
            mcf.SerializeSection(writer);
            writer.Close();
            */
            /*
            ModConfigSection mcf = new ModConfigSection();
            for(var i = 0; i < modules.Count; i++)
                mcf.ModuleConfigCollection[i] = modules[i];

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter tw = new XmlTextWriter(sw);

            tw.Formatting = Formatting.Indented;

            mcf.SerializeSection(tw);
            tw.Close();
             */
            /*XmlReader reader = XmlReader.Create(new StreamReader(@"f:/file.xml"));
            reader.MoveToContent();
            ModConfigSection rmcf = new ModConfigSection();
            rmcf.DeserializeSection(reader);
            reader.Close();*/

            //File.AppendAllText(@"f:/file.xml", sb.ToString());

            //Console.WriteLine(sb.ToString());

            string saveFolder = @"f:/save_file";
            string oldSave = @"/001";
            string newSave = @"/002";
            string oldFile = saveFolder + oldSave + "/001.config";
            string newFile = saveFolder + newSave + "/002.config";

            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();

            configMap.ExeConfigFilename = oldFile;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            ModConfigSection mm = config.GetSection("ModConfigSection") as ModConfigSection;

            System.IO.Directory.CreateDirectory(saveFolder + newSave);
            config.SaveAs(newFile, ConfigurationSaveMode.Minimal);
            Console.ReadLine();
        }
    }
}
