using Mod.Configuration.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Configuration.Types
{
    public class TypeFabrication: IComparable<TypeFabrication>
    {
        #region variables
        private string assembly = "";
        private Type baseType = null;
        private Type createType = null;
        private ConfigureAttribute config = null;
        private string culture = "";
        private int genericCnt = 0;
        private List<TypeFabrication> genericTypes = new List<TypeFabrication>();
        private List<TypeFabrication> nestedTypes = new List<TypeFabrication>();
        private bool isNested = false;
        private string name = "";
        private string namespaceString = "";
        private string publicKeyToken = "";
        private string version = "";
        #endregion

        #region properties
        public string Assembly
        {
            get { return assembly; }
            set { assembly = value; }
        }

        public string Culture
        {
            get { return culture; }
            set { culture = value; }
        }
        public Type BaseType
        {
            get { return baseType; }
            set { baseType = value; }
        }

        public Type CreateType
        {
            get { return createType; }
            set { createType = value; }
        }
        public ConfigureAttribute Config
        {
            get { return config; }
            set { config = value; }
        }

        public string FullName
        {
            get
            {
                string result = Name;
                if(Namespace != "")
                    result = Namespace + "." + Name;
                return result;
            }
        }

        public int GenericCnt
        {
            get { return genericCnt; }
            set { genericCnt = value; }
        }

        public List<TypeFabrication> GenericTypes
        {
            get { return genericTypes; }
        }

        public bool IsGeneric
        {
            get { return genericCnt > 0; }
        }

        public bool IsNested
        {
            get { return isNested; }
            set { isNested = value; }
        }

        public string Name
        {
            get
            {
                string result = name;
                if(GenericCnt > 0)
                    result += "`" + GenericCnt.ToString();
                return result;
            }
            set { name = value; }
        }

        public string Namespace
        {
            get { return namespaceString; }
            set { namespaceString = value; }
        }

        public List<TypeFabrication> NestedTypes
        {
            get { return nestedTypes; }
        }

        public string PublicKeyToken
        {
            get { return publicKeyToken; }
            set { publicKeyToken = value; }
        }

        public string Version
        {
            get { return version; }
            set { version = value; }
        }
        #endregion
        #region functions
        public bool CanInstantiate(bool isCheckOnly = true)
        {
            if(!isCheckOnly)
            {
                if(baseType == null)
                    FindBaseType();
                if(FindCreateType())
                    return true;
            }

            if(baseType != null)
            {
                if((CreateType == null) && !isCheckOnly)
                    if(FindCreateType())
                        return true;
            }

            return false;
        }

        public int CompareTo(TypeFabrication other)
        {
            return -1;
        }

        public object Fabricate()
        {
            if(CreateType != null)
            {
                return Activator.CreateInstance(CreateType);
            }
            return null;
        }

        public bool FindBaseType()
        {
            bool result = baseType != null;
            if(BaseType == null)
            {
                string tmpName = Name;

                if(GenericCnt > 0)
                    tmpName += "`" + GenericCnt;

                if(Namespace.Length > 0)
                    tmpName = Namespace + "." + tmpName;

                BaseType = Type.GetType(tmpName);
            }
            if(BaseType != null)
            {
                Config = BaseType.GetCustomAttribute(typeof(ConfigureAttribute)) as ConfigureAttribute;
                return true;
            }
            return false;
        }

        public bool FindCreateType()
        {
            if(BaseType != null)
            {
                if(IsGeneric)
                {
                    if(GenericTypes.Count() == GenericCnt)
                    {
                        try
                        {
                            Type[] genericArray = GetGenericsAsArray();
                            if(genericArray.Count() == GenericCnt)
                            {
                                CreateType = BaseType.MakeGenericType(genericArray);
                                return true;
                            }
                        }
                        catch
                        {
                            CreateType = null;
                        }
                    }
                }
                else
                {
                    CreateType = BaseType;
                }
                if((CreateType == null) && (Config != null))
                {
                    CreateType = Config.InitType;
                }

            }
            return CreateType != null;
        }

        public Type[] GetGenericsAsArray()
        {
            List<Type> generics = new List<Type>();
            for(int i = 0; i < GenericTypes.Count(); i++)
            {
                if(!GenericTypes[i].FindBaseType())
                    GenericTypes[i].BaseType = TypeFactory.GetInstance().FindBaseType(GenericTypes[i]);
                if((GenericTypes[i].BaseType != null) && GenericTypes[i].FindCreateType())
                    generics.Add(GenericTypes[i].CreateType);
                else
                    break;
            }
            return generics.ToArray();
        }
        #endregion

    }

}
