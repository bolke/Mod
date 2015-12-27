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
        static void SaveAndLoadTest()
        {
            ModConfigSectionReader mcr = new ModConfigSectionReader();
            ModConfigSectionWriter mcw = new ModConfigSectionWriter();

            mcr.Load();
            ModConfigSection mcs = mcr.Section;
            mcw.ModConfigSection = mcs;

            mcw.Save("saved.xml");

            mcr = new ModConfigSectionReader();
            mcr.LoadFromFile("./saved.xml");
        }

        static void Main(string[] args)
        {
            ModConfigSectionReader reader = new ModConfigSectionReader();
            reader.Load();
            Console.ReadLine();
        }
    }
}
