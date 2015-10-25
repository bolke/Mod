using System.Configuration;
namespace ModBase.Interfaces
{
  public interface IConfigurable
  {
    ConfigurationElement ToConfig();
    bool FromConfig(ConfigurationElement config);
  }
}
