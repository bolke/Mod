using ModBase.Configuration.Properties;
using ModBase.Interfaces;
using ModBase.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModCombi.Modules
{
    public class ThreadRunnable : Runnable
    {
        private Thread thread = null;
        private uint interval = 1;

        [Configure("Interval", DefaultValue=1)]
        public virtual uint Interval
        {
            get
            {
                lock (Padlock) return interval;
            }
            set
            {
                lock(Padlock) interval = value;
            }
        }    

        public override bool Start()
        {
            bool result = false;
            lock (Padlock)
            {
                result = base.Start();
                if (result)
                {
                    if (Delegated != null)
                    {
                        thread = new Thread(new ThreadStart(Run));
                        thread.Start();
                    }
                    else
                    {
                        lock (Padlock)
                        {
                            IsStarted = false;
                            IsRunning = false;
                            result = false;
                        }
                    }
                }
            }
            return result;
        }

        public void Run()
        {
            while (IsStarted)
            {                                   
                if(IsRunning){
                    Delegate run = null;
                    object[] param = null;
                    lock (Padlock)
                    {
                        run = Delegated;
                        param = DelegateParameters;
                    }

                    if (run != null)
                        run.DynamicInvoke(param);
                    Thread.Sleep(Convert.ToInt32(Interval));
                }
                else
                    Thread.Sleep(Convert.ToInt32(Interval));
            }
        }
        
    }
}
