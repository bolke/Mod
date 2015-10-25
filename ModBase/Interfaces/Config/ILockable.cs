using ModBase.Configuration.Properties;

namespace ModBase.Interfaces
{
  public interface ILockable : IInitiator
  {
    [Configure(PreInit = true, InitType = typeof(object))]
    object Padlock
    {
      get;
      set;
    }

    bool IsLocked
    {
      get;
    }

    bool Lock();
    bool Unlock();
  }
}
