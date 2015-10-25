using ModBase.Configuration.Properties;
using ModBase.Interfaces;
using ModBase.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ModCombi.Modules
{
    public class TimerRunnable : Runnable
    {
        private System.Timers.Timer timer = new System.Timers.Timer();        
        private uint interval = 1;
        private ElapsedEventHandler eventHandler = null;

        [Configure("Interval", DefaultValue = 1)]
        public virtual uint Interval
        {
            get
            {
                lock (Padlock) return interval;
            }
            set
            {
                lock (Padlock) interval = value;
            }
        }      

        public virtual ElapsedEventHandler EventHandler
        {
            get { lock (Padlock) return eventHandler; }
            set
            {
                lock (Padlock)
                {
                    if (!IsRunning)
                        eventHandler = value;
                }
            }
        }

        public override bool Start()
        {
            lock (Padlock)
            {
                if (base.Start())
                {
                    if (EventHandler != null)
                        timer.Elapsed += EventHandler;
                    timer.Elapsed += ElapsedEventReset;
                    timer.AutoReset = false;                    
                    timer.Interval = Interval;
                    timer.Start();
                    return true;
                }
            }
            return false;
        }

        public override Boolean Stop()
        {
            lock (Padlock)
            {
                if (base.Stop())
                {
                    timer.Stop();
                    if (EventHandler != null)
                        timer.Elapsed -= EventHandler;
                    timer.Elapsed -= ElapsedEventReset;
                    return true;
                }
            }
            return false;
        }

        public override bool Pause()
        {
            lock (Padlock)
            {
                if (base.Pause())
                {
                    timer.Stop();
                    return true;
                }
            }
            return false;
        }

        public override bool Resume()
        {
            lock (Padlock)
            {
                if (base.Resume())
                {
                    timer.Start();
                    return true;
                }
            }
            return false;
        }

        public void Run()
        {
            if (IsStarted){
                if (IsRunning)
                {
                    Delegate run = null;
                    object[] param = null;
                    lock (Padlock)
                    {
                        run = Delegated;
                        param = DelegateParameters;
                    }

                    if (run != null)
                        run.DynamicInvoke(param);                    
                }
            }
        }

        protected virtual void ElapsedEventReset(object sender, ElapsedEventArgs e)
        {
            Run();
            if (IsStarted)
            {
                lock (Padlock)
                {
                    if (!IsStopped)
                    {
                        timer.Interval = Interval;
                        timer.Start();
                    }
                }
            }
        }

    }
}
