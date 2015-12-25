using Mod.Configuration.Properties;

namespace Mod.Interfaces
{
    public interface ILockable: IInitiator
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
