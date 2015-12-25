using Mod.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mod.Modules.Abstracts
{
    public abstract class Lockable: Initiator, ILockable
    {
        private object padlock = new Object();

        public virtual object Padlock
        {
            get
            {
                return padlock;
            }
            set
            {
                if(value != null)
                {
                    if(padlock != null)
                        lock(padlock)
                            padlock = value;
                    else
                        padlock = value;
                }
            }
        }

        public virtual bool IsLocked
        {
            get
            {
                if(Monitor.TryEnter(Padlock))
                {
                    Unlock();
                    return false;
                }
                return true;
            }
        }

        public Lockable()
        {
        }

        public override bool Initialize()
        {
            if(base.Initialize())
            {
                padlock = new Object();
                return true;
            }
            return false;
        }

        public override bool Cleanup()
        {
            if(IsInitialized)
            {
                padlock = new Object();
                return base.Cleanup();
            }
            return false;
        }

        public virtual bool Lock()
        {
            bool result = false;
            Monitor.Enter(Padlock, ref result);
            return result;
        }

        public virtual bool Unlock()
        {
            Monitor.Exit(Padlock);
            return true;
        }
    }
}
