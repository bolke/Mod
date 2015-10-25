using ModBase.Configuration.Properties;
using ModBase.Interfaces;
using ModBase.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCombi.Modules
{
    [Configure(InitType = typeof(RunnableWorker<IJob>))]
    public class RunnableWorker<T>: Initiator, IRunnable, IWorker<T> where T:IJob
    {
        private IWorker<T> worker = null;
        private IRunnable runnable = null;
        private ILockable lockable = null;

        private delegate bool DoWork();

        [Configure("Runnable", InitType = typeof(Runnable))]
        public virtual IRunnable Runnable
        {
            get { return runnable; }
            set { runnable = value; }
        }

        [Configure("Worker", InitType = typeof(Worker<>))]
        public virtual IWorker<T> Worker
        {
            get { return worker; }
            set { worker = value; }
        }

        [Configure("Lockable", InitType = typeof(Lockable))]
        public virtual ILockable Lockable
        {            
            get { return lockable; }
            set { lockable = value; }
        }

        public virtual object Padlock
        {
            get
            {
                try
                {
                    return Lockable.Padlock;
                }
                catch
                {
                    return new object();
                }
            }
            set
            {
                Lockable.Padlock = value;
            }
        }   

        public virtual Delegate Delegated
        {
            get
            {
                lock (Padlock) return runnable.Delegated;
            }
            set
            {
                if (!IsRunning)
                {
                    lock (Padlock)
                    {
                        runnable.Delegated = value;
                    }
                }
            }
        }

        public virtual object[] DelegateParameters
        {
            get
            {
                lock (Padlock) return runnable.DelegateParameters;
            }
            set
            {
                lock (Padlock) runnable.DelegateParameters = value;
            }
        }

        public virtual bool IsStarted
        {
            get { return Runnable.IsStarted; }
            set { Runnable.IsStarted = value; }
        }

        public virtual bool IsStopped
        {
            get { return Runnable.IsStopped; }
            set { Runnable.IsStopped = value; }
        }

        public virtual bool IsRunning
        {
            get { return Runnable.IsRunning; }
            set { Runnable.IsRunning = value; }
        }

        public virtual bool IsPaused
        {
            get { return Runnable.IsPaused; }
            set { Runnable.IsPaused = value; }
        }

        public virtual bool AutoStart
        {
            get { return Runnable.AutoStart; }
            set { Runnable.AutoStart = value; }
        }

        public virtual bool Start()
        {
            return Runnable.Start();
        }

        public virtual bool Pause()
        {
            return Runnable.Pause();
        }

        public virtual bool Resume()
        {
            return Runnable.Resume();
        }

        public virtual bool Stop()
        {
            return Runnable.Stop();
        }

        public virtual IPipe<T> Pool
        {
            get { return Worker.Pool; }
            set { Worker.Pool = value; }
        }

        public virtual IPipe<T> Finished
        {
            get { return Worker.Finished; }
            set { Worker.Finished = value; }
        }

        public virtual bool Work()
        {            
            if(Runnable.IsRunning)            
                 return Worker.Work();
            return false;
        }

        public override bool Initialize()
        {
            if(base.Initialize())
            {
                Runnable.Delegated = new DoWork(Work);
                return true;
            }
            return false;
        }

        public virtual void Dispose()
        {            
        }
    }
}
