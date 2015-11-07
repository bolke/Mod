using Mod.Configuration.Section;
using Mod.Modules;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModConsole
{
  class Program
  {
    static void Main(string[] args)
    {
      SerialPort comPort = new SerialPort("COM11");

      comPort.BaudRate = 9600;
      comPort.StopBits = StopBits.One;
      comPort.DataBits = 8;

      comPort.ReadTimeout = 10;
      comPort.Open();

      StreamPipe<byte[]> knutty = new StreamPipe<byte[]>();
      knutty.Initialize();

      knutty.Stream = comPort.BaseStream;

      knutty.Push("0123456789".ToCharArray() as byte[]);

      while(true)
      {
        Console.WriteLine(knutty.Pop());
        Thread.Sleep(1000);
      }

      ModConfigSectionReader loader = new ModConfigSectionReader();
      loader.LoadAndRun();
      Console.ReadLine();
    }
  }
}
