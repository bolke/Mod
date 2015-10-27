using Mod.Configuration.Modules;
using System.Configuration;
namespace Mod.Interfaces
{
  public interface IConfigurable
  {
    ModuleConfig ToConfig();
    bool FromConfig(ModuleConfig config);
  }
}
