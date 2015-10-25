using ModBase.Configuration.Properties;
namespace ModBase.Interfaces
{
  public interface IInitiator
  {
    [Configure(DefaultValue = false)]
    bool AutoCleanup
    {
      get;
      set;
    }

    [Configure(DefaultValue = false)]
    bool IsInitialized
    {
      get;
      set;
    }

    [Configure(DefaultValue = false)]
    bool AutoInitialize
    {
      get;
      set;
    }

    bool Initialize();
    bool Cleanup();
  }
}
