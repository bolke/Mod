using ModBase.Configuration.Properties;
using ModBase.Configuration.Modules;
using ModBase.Configuration.Plugins;
using ModBase.Configuration.Types;
using ModBase.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModBase.Configuration.Section
{
  public class ModConfigSectionLoader
  {
    #region variables
    private ModuleConfigCollection modules = null;

    private Dictionary<Guid, ModuleConfig> moduleInstances = new Dictionary<Guid, ModuleConfig>();
    private Boolean modulesLoaded = false;
    private Dictionary<Guid, ModuleConfig> moduleReferences = new Dictionary<Guid, ModuleConfig>();    
    private PluginConfigCollection plugins = null;
    private Boolean pluginsLoaded = false;
    private Dictionary<Guid, PropertyConfig> propertyReferences = new Dictionary<Guid, PropertyConfig>();
    private TypeFactory typeFactory = TypeFactory.GetInstance();
    #endregion

    #region properties
    public ModuleConfigCollection Modules
    {
      get
      {
        return modules;
      }

      protected set
      {
        modules = value;
      }
    }

    public Dictionary<Guid, ModuleConfig> ModuleInstances
    {
      get
      {
        return moduleInstances;
      }
    }

    public Dictionary<Guid, ModuleConfig> ModuleReferences
    {
      get
      {
        return moduleReferences;
      }
    }

    public Dictionary<Guid, PropertyConfig> PropertyReferences
    {
      get
      {
        return propertyReferences;
      }
    }

    public PluginConfigCollection Plugins
    {
      get
      {
        return plugins;
      }
      protected set
      {
        plugins = value;
      }
    }
    #endregion

    #region constructor
    public ModConfigSectionLoader()
    {

    }
    #endregion

    #region functions
    protected virtual Boolean ConfigureInstances()
    {
      bool result = true;
      Dictionary<Guid, ModuleConfig>.Enumerator modInstEnum = moduleInstances.GetEnumerator();
      while (modInstEnum.MoveNext())
      {
        ModuleConfig module = modInstEnum.Current.Value;
        if (module != null)
          result = result && module.Initialize();
        else
        {
          result = false;
          break;
        }
      }
      return result;
    }

    protected virtual Boolean ConnectReferences()
    {
      bool result = false;
      Dictionary<Guid, ModuleConfig>.Enumerator modInstEnum = moduleInstances.GetEnumerator();
      while (modInstEnum.MoveNext())
      {
        result = true;
        if (modInstEnum.Current.Value.HasProperty("Key"))
        {
          ModuleConfig instance = modInstEnum.Current.Value;
          Dictionary<Guid, ModuleConfig>.Enumerator modRefEnum = moduleReferences.GetEnumerator();
          while (modRefEnum.MoveNext())
          {
            if (instance.GetConfigurationProperty("Key").Equals(modRefEnum.Current.Value.GetConfigurationProperty("Key")))
              modRefEnum.Current.Value.Instance = instance.Instance;
          }
        }
      }
      return result;
    }

    protected virtual Boolean ConnectPropertyReferences()
    {
      bool result = false;
      Dictionary<Guid, PropertyConfig>.Enumerator propInstEnum = propertyReferences.GetEnumerator();
      while (propInstEnum.MoveNext())
      {
        result = true;
        PropertyConfig instance = propInstEnum.Current.Value;
        if(instance.Original){          
          Dictionary<Guid, PropertyConfig>.Enumerator propRefEnum = propertyReferences.GetEnumerator();          
          while (propRefEnum.MoveNext())
          {
            if (propRefEnum.Equals(propInstEnum))
              continue;
            if (instance.Key == propRefEnum.Current.Value.Key)
            {
              propRefEnum.Current.Value.OriginalConfig = instance;
              instance.PropertyMembers.Add(propRefEnum.Current.Value);
            }              
          }
        }
      }
      return result;
    }

    protected virtual Boolean CreateInstance(ModuleConfig config)
    {      
      TypeFabrication moduleType = typeFactory.Fabricate(config.Type);
      moduleType.BaseType = typeFactory.FindBaseType(moduleType);
      if (moduleType.FindBaseType() && moduleType.FindCreateType())
      {
        config.Instance = moduleType.Fabricate();
      }
      IEnumerator enumerator = config.PropertyConfigCollection.GetEnumerator();
      while (enumerator.MoveNext())
      {
        PropertyConfig propertyConfig = enumerator.Current as PropertyConfig;
        propertyReferences[propertyConfig.UniqueId] = propertyConfig;
        propertyConfig.Parent = config;
      }
      return config.Instance != null;
    }

    protected virtual Boolean CreateInstances(IEnumerator moduleEnum, int depth, ModuleConfig parent = null)
    {
      bool result = true;
      while (moduleEnum.MoveNext())
      {
        ModuleConfig moduleConfig = moduleEnum.Current as ModuleConfig;
        if (moduleConfig != null)
        {
          if (moduleConfig.IsInstance())
          {
            moduleConfig.Parent = parent;
            moduleInstances[moduleConfig.UniqueId] = moduleConfig;
            result = LoadModules(moduleConfig, depth + 1) && result;
            result = LoadModule(moduleConfig) && result;
          }
          else
            moduleReferences[moduleConfig.UniqueId] = moduleConfig;
        }
      }
      return result;
    }

    protected virtual Boolean FillPipes()
    {
      bool result = true;
      Dictionary<Guid, ModuleConfig>.Enumerator modInstEnum = moduleInstances.GetEnumerator();
      Dictionary<Guid, ModuleConfig>.Enumerator modRefEnum = moduleReferences.GetEnumerator();
      ModuleConfig module = null;
      while (modInstEnum.MoveNext())
      {
        module = modInstEnum.Current.Value;
        if (module.IsPipe())
        {
          IBasePipe bucket = module.Instance as IBasePipe;
          IEnumerator pipeContentEnum = module.ModuleConfigCollection.GetEnumerator();
          while (pipeContentEnum.MoveNext())
          {
            ModuleConfig pipeContent = pipeContentEnum.Current as ModuleConfig;
            if ((pipeContent != null) && (!pipeContent.HasProperty("Property", false)))
              bucket.PushObject(pipeContent.Instance);
          }
        }
      }

      while (modRefEnum.MoveNext())
      {
        module = modRefEnum.Current.Value;
        if (module.IsPipe())
        {
          IBasePipe bucket = module.Instance as IBasePipe;
          IEnumerator pipeContentEnum = module.ModuleConfigCollection.GetEnumerator();
          while (pipeContentEnum.MoveNext())
          {
            ModuleConfig pipeContent = pipeContentEnum.Current as ModuleConfig;
            if ((pipeContent != null) && (!pipeContent.HasProperty("Property", false)))
              bucket.PushObject(pipeContent.Instance);
          }
        }
      }
      return result;
    }

    protected Boolean FillAttributes()
    {
      bool result = true;
      Dictionary<Guid, ModuleConfig>.Enumerator modInstEnum;
      modInstEnum = moduleInstances.GetEnumerator();
      ModuleConfig module = null;
      while (modInstEnum.MoveNext())
      {
        module = modInstEnum.Current.Value as ModuleConfig;
        result = result && FillAttributes(module);
      }
      return result;
    }

    protected Boolean FillAttributes(ModuleConfig module)
    {
      bool result = true;
      IEnumerator modInstEnum = module.ModuleConfigCollection.GetEnumerator();
      ModuleConfig propertyModule = null;
      while (modInstEnum.MoveNext())
      {
        propertyModule = modInstEnum.Current as ModuleConfig;
        if (propertyModule != null)
        {
          result = result && FillAttributes(propertyModule);
          if (propertyModule.HasProperty("Property", false))
            result = result && module.SetInstanceProperty(propertyModule);
        }
        else
          result = false;
      }
      return result;
    }
    
    protected virtual void FindParentGenerics(ModuleConfig config, TypeFabrication moduleType)
    {
      ModuleConfig parentConfig = config.Parent;
      while (moduleType.GenericTypes.Count < moduleType.GenericCnt)
      {
        if (parentConfig == null)
          break;

        TypeFabrication parentType = typeFactory.Fabricate(parentConfig.Type);
        if (parentType.GenericTypes.Count == moduleType.GenericCnt)
        {
          moduleType.GenericTypes.AddRange(parentType.GenericTypes);
          break;
        }

        parentConfig = parentConfig.Parent;
      }
    }

    protected virtual Boolean FindInstance(ModuleConfig config)
    {
      if (modules != null)
      {
        IEnumerator enumerator = modules.GetEnumerator();
        if (enumerator != null)
        {
          while (enumerator.MoveNext())
          {
            ModuleConfig instance = enumerator.Current as ModuleConfig;
            if (instance != null && instance.IsInstance())
            {
              if (instance.HasProperty("Key") && instance.GetConfigurationProperty("Key") == config.GetConfigurationProperty("Key"))
              {
                config.Instance = instance.Instance;
                return config.Instance != null;
              }
            }
          }
        }
      }
      return false;
    }

    protected virtual Boolean LoadModule(ModuleConfig config)
    {
      if ((config != null) && config.IsInstance())
        return CreateInstance(config);
      return false;
    }

    protected virtual Boolean LoadModules(ModuleConfigCollection collection, int depth)
    {
      Boolean result = false;
      if (collection != null)
      {
        IEnumerator moduleEnum = collection.GetEnumerator();
        if (moduleEnum != null)
        {
          result = CreateInstances(moduleEnum, depth);
          if (depth == 0)
          {
            modules = collection;
            result = result && ConnectPropertyReferences();
            result = result && ConfigureInstances();
            result = result && ConnectReferences();            
            result = result && FillAttributes();
            result = result && FillProperties();
            result = result && FillPipes();
          }
        }
      }
      return result;
    }

    protected virtual Boolean FillProperties()
    {
      bool result = true;
      Dictionary<Guid, ModuleConfig>.Enumerator modInstEnum;
      modInstEnum = moduleInstances.GetEnumerator();
      ModuleConfig module = null;
      while (modInstEnum.MoveNext())
      {
        module = modInstEnum.Current.Value as ModuleConfig;
        result = result && module.SetLonghandProperties();
      }
      return result;
    }

    protected virtual Boolean LoadModules(ModuleConfig parent, int depth)
    {
      ModuleConfigCollection collection = parent.ModuleConfigCollection;
      Boolean result = false;
      if (collection != null)
      {
        IEnumerator moduleEnum = collection.GetEnumerator();
        if (moduleEnum != null)
        {
          result = CreateInstances(moduleEnum, depth, parent);
          if (depth == 0)
          {
            modules = collection;
            result = result && ConnectPropertyReferences();
            result = result && ConfigureInstances();
            result = result && ConnectReferences();
            result = result && FillAttributes();
            result = result && FillProperties();
            result = result && FillPipes();
          }
        }
      }
      return result;
    }

    protected virtual Boolean LoadPlugins(PluginConfigCollection collection)
    {
      if (collection != null)
      {
        IEnumerator pluginEnum = collection.GetEnumerator();
        if (pluginEnum != null)
        {
          while (pluginEnum.MoveNext())
          {
            PluginConfig pluginConfig = pluginEnum.Current as PluginConfig;
            if ((pluginConfig != null) && (pluginConfig.Assembly != null))
              continue;
            else
              throw new ConfigurationErrorsException();
          }
          plugins = collection;
          return true;
        }
      }
      return false;
    }

    public Boolean LoadAndRun(string sectionName = "ModConfigSection")
    {
      if(Load(sectionName))
        return Run();
      return false;
    }

    public Boolean Load(string sectionName = "ModConfigSection")
    {
      bool result = false;
      //!TODO cleaning of private variables, making it ready for loading, removing old loaded instances
      typeFactory.DeepSearch = true;
      typeFactory.LoadPreloadedAssemblies();

      System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      ModConfigSection section = configuration.GetSection(sectionName) as ModConfigSection;
      if (section != null)
      {
        pluginsLoaded = LoadPlugins(section.PluginCollection);
        if (pluginsLoaded)
        {
          modulesLoaded = LoadModules(section.ModuleConfigCollection, 0);
          if (modulesLoaded)
          {
            for (int i = 0; i < modules.Count; i++)
            {
              IInitiator initiator = modules[i].Instance as IInitiator;
              if (initiator != null)
                if ((initiator.Initialize() == false) && !initiator.IsInitialized)
                  throw new ConfigurationErrorsException();
            }
            result = true;
          }
        }
      }
      return result;
    }

    public Boolean Run()
    {
      for (int i = 0; i < moduleInstances.Count; i++)
      {
        IRunnable runnable = moduleInstances.ElementAt(i).Value.Instance as IRunnable;
        if ((runnable != null) && (runnable.AutoStart))
          runnable.Start();
      }
      return true;
    }
    #endregion

  }
}
