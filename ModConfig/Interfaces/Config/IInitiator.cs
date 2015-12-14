﻿using Mod.Configuration.Properties;
using System;
namespace Mod.Interfaces
{
  public interface IInitiator
  {
    [Configure(InitType = typeof(Guid))]
    Guid UniqueId
    {
      get;
      set;
    }

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
