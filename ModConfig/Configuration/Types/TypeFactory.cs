using Mod.Configuration.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Configuration.Types
{
  public class TypeFactory
  {
    #region variables
    #region static variables
    private static TypeFactory instance = null;
    #endregion
    private bool deepSearch = false;
    private List<Assembly> pluginAssemblies = new List<Assembly>();
    private List<Assembly> preloadedAssemblies = new List<Assembly>();
    #endregion

    #region constructors
    public static TypeFactory GetInstance()
    {
      if (instance == null)
        instance = new TypeFactory();
      return instance;
    }

    private TypeFactory()
    {

    }
    #endregion

    #region properties
    public bool DeepSearch
    {
      get { return deepSearch; }
      set { deepSearch = value; }
    }

    public List<Assembly> PluginAssemblies
    {
      get { return pluginAssemblies; }
    }

    public List<Assembly> PreloadedAssemblies
    {
      get { return preloadedAssemblies; }
    }

    #endregion
    #region functions
    protected void FillFabricationWithElements(TypeFabrication result, string[] elements)
    {
      for (int i = 0; i < elements.Count(); i++)
      {
        elements[i] = elements[i].Trim();
        if (elements[i].StartsWith("Version="))
          result.Version = elements[i].Replace("Version=", "").Trim();
        else if (elements[i].StartsWith("Culture="))
          result.Culture = elements[i].Replace("Culture=", "").Trim();
        else if (elements[i].StartsWith("PublicKeyToken="))
          result.PublicKeyToken = elements[i].Replace("PublicKeyToken=", "").Trim();
        else if (i == 1)
        {
          result.Assembly = elements[i].Trim();
        }
        else if (i == 0)
        {
          if (elements[i].Contains('.'))
          {
            result.Name = elements[i].Substring(elements[i].LastIndexOf('.') + 1).Trim();
            result.Namespace = elements[i].Substring(0, elements[i].LastIndexOf('.')).Trim();
          }
          else
            result.Name = elements[i].Trim();
          if (result.Name.Contains("`"))
            result.Name = result.Name.Substring(0, result.Name.IndexOf("`"));
        }
      }
    }

    protected Type FindInAssembly(TypeFabrication fab, Assembly assembly)
    {
      IEnumerator<TypeInfo> et = assembly.DefinedTypes.GetEnumerator();
      while (et.MoveNext())
      {
        if (et.Current.Name.ToLower().Equals(fab.Name.ToLower()))
          return et.Current.AsType();
      }
      return null;
    }

    protected Type FindInAssemblyList(TypeFabrication fab, IEnumerator<Assembly> enumerator)
    {
      Type result = null;
      bool deepSearch = DeepSearch && (String.IsNullOrWhiteSpace(fab.Namespace));
      bool findAssembly = fab.Assembly != "";

      while ((result == null) && (enumerator.MoveNext()))
      {
        Assembly assembly = enumerator.Current;
        if (assembly.GetName().Name == fab.Assembly)
          result = assembly.GetType(fab.FullName);
        else
        {
          result = assembly.GetType(fab.FullName);
          if ((result == null) && (deepSearch))
            result = FindInAssembly(fab, assembly);
        }
      }
      return result;
    }
    protected string FindGenericCnt(string def)
    {
      string result = "0";
      if ((def != null) && def.Contains('`'))
      {
        result = def.Substring(def.IndexOf('`') + 1);
        for (int i = 0; i < result.Count(); i++)
        {
          if (result[i] < '0' || result[i] > '9')
          {
            result = result.Substring(0, i);
            break;
          }
        }
      }
      return result;
    }

    protected List<String> SplitDefTypes(string def)
    {
      List<String> result = new List<String>();
      string left = null;
      string middle = null;
      string right = null;
      int depth = 0;
      int start = 0;
      int stop = 0;

      if (def.Contains('[') && def.Contains(']'))
      {
        start = def.IndexOf('[');
        stop = def.LastIndexOf(']');
        left = def.Substring(0, start);
        middle = def.Substring(start + 1, stop - start - 1);
        right = def.Substring(stop + 1);
        def = left + right;
        start = 0;
        for (int i = 0; i < middle.Count(); i++)
        {
          if (middle[i] == '[')
          {
            if (depth == 0)
              start = i;
            depth++;
          }
          if (middle[i] == ']')
          {
            depth--;
            if (depth == 0)
            {
              result.Add(middle.Substring(start + 1, i - start - 1));
              start = i + 1;
            }
          }
        }
        result.Insert(0, left + right);
      }
      return result;
    }
    public TypeFabrication Fabricate(string def)
    {
      TypeFabrication result = new TypeFabrication();
      List<String> genericDefs = null;
      string cnt = "0";

      genericDefs = this.SplitDefTypes(def);
      if (genericDefs.Count > 0)
      {
        def = genericDefs.First();
        genericDefs.RemoveAt(0);
      }

      cnt = FindGenericCnt(def);
      List<TypeFabrication> generics = new List<TypeFabrication>();
      for (int i = 0; i < genericDefs.Count; i++)
        generics.Add(Fabricate(genericDefs[i]));

      string[] elements = def.Split(',');
      FillFabricationWithElements(result, elements);

      result.GenericCnt = Convert.ToInt32(cnt);
      result.GenericTypes.Clear();
      result.GenericTypes.AddRange(generics);

      result.CanInstantiate(false);
      return result;
    }

    public TypeFabrication Fabricate(Type baseType)
    {
      TypeFabrication result = Fabricate(baseType.AssemblyQualifiedName);
      result.BaseType = baseType;
      return result;
    }

    public Type FindBaseType(TypeFabrication fab)
    {
      Type result = null;
      if (fab != null)
      {
        bool findAssembly = fab.Assembly != "";
        result = FindInAssemblyList(fab, pluginAssemblies.AsEnumerable().GetEnumerator());
        if (result == null)
          result = FindInAssemblyList(fab, preloadedAssemblies.AsEnumerable().GetEnumerator());
      }
      return result;
    }
    public void LoadPreloadedAssemblies()
    {
      if (preloadedAssemblies.Count == 0)
      {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        PreloadedAssemblies.AddRange(assemblies.AsEnumerable());
      }
    }

    #endregion

  }

}
