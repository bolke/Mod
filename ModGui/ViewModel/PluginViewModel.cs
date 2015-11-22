using Mod.Configuration.Plugins;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModGui.ViewModel
{
  public class PluginViewModelCollection
  {
    public ObservableCollection<PluginViewModel> plugins = new ObservableCollection<PluginViewModel>();
  }

  public class PluginViewModel
  {
    protected PluginConfig config
    {
      get;
      set;
    }

    public string Name
    {
      get { return IsLoaded?config.Assembly.FullName:""; }      
    }

    public string File
    {
      get { return config.File; }
    }

    public string Path
    {
      get { return config.Path; }
    }

    public bool IsLoaded
    {
      get { return config.Assembly != null; }
    }
  }
}
